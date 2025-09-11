using UnityEngine;
using System.Threading.Tasks;
using System;
using Io.Rollout.Rox.Client;
using Io.Rollout.Rox.Core.Entities;
using Io.Rollout.Rox.Client.Flags;

namespace KeepDriving.CloudBees
{
    public class CarFlags : IRoxContainer 
    {
        public RoxString carColor = new RoxString("Blue", new string[] 
        { "Blue", "Red", "Green", "Yellow", "White", "Black", "Purple", "Orange" });
        public RoxFlag enableTutorial = new RoxFlag(false);
        public RoxInt maxSpeed = new RoxInt(120, new int[] {80, 100, 120, 150, 200});
        public RoxDouble fuelConsumption = new RoxDouble(1.0, new double[] {0.5, 0.8, 1.0, 1.2, 1.5});
        public RoxFlag enableNightMode = new RoxFlag(false);
        public RoxFlag enableNewCarModel = new RoxFlag(false);
    }

    public class CloudBeesManager : MonoBehaviour
    {
        [Header("CloudBees Configuration")]
        // ⚠️ on supprime la clé en dur et on ne sérialise plus
        private string sdkKey = null;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool forceOfflineMode = false;

        public static CarFlags Flags { get; private set; }
        public static bool IsInitialized { get; private set; } = false;
        public static CloudBeesManager Instance { get; private set; }

        public static event System.Action OnFlagsReady;
        public static event System.Action<string> OnCarColorChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            { Destroy(gameObject); return; }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 1) tenter depuis variable d'environnement (utile dans l'Editor / CI)
            sdkKey = Environment.GetEnvironmentVariable("SDK_KEY");

            // 2) sinon depuis Resources (injecté par Jenkins)
            if (string.IsNullOrWhiteSpace(sdkKey))
            {
                var txt = Resources.Load<TextAsset>("cloudbees_key"); // Assets/Resources/cloudbees_key.txt
                if (txt != null) sdkKey = txt.text.Trim();
            }

            if (!forceOfflineMode)
                InitializeCloudBees();
            else
                InitializeOfflineMode();
        }

        private async void InitializeCloudBees()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sdkKey))
                    throw new Exception("SDK key not found (env: SDK_KEY or Resources/cloudbees_key.txt).");

                Log("🚀 Initializing CloudBees Feature Flags...");
                Flags = new CarFlags();
                Rox.Register("", Flags);

                var options = new RoxOptions(new RoxOptions.RoxOptionsBuilder {});

                await Rox.Setup(sdkKey, options);

                IsInitialized = true;
                Log("✅ CloudBees initialized successfully!");
                LogAllFlags();
                OnFlagsReady?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ CloudBees initialization failed: {ex.Message}");
                InitializeOfflineMode();
            }
        }

        private void InitializeOfflineMode()
        {
            Log("🔧 Initializing offline mode with default values...");
            Flags = new CarFlags();
            IsInitialized = true;
            Log("✅ Offline mode initialized!");
            LogAllFlags();
            OnFlagsReady?.Invoke();
        }

        private void LogAllFlags()
        {
            if (!IsInitialized || Flags == null) return;
            Log("🎯 Current Feature Flag Values:");
            Log($"  🚗 Car Color: {Flags.carColor.GetValue()}");
            Log($"  📚 Enable Tutorial: {Flags.enableTutorial.IsEnabled()}");
            Log($"  ⚡ Max Speed: {Flags.maxSpeed.GetValue()} km/h");
            Log($"  ⛽ Fuel Consumption: {Flags.fuelConsumption.GetValue()}x");
            Log($"  🌙 Night Mode: {Flags.enableNightMode.IsEnabled()}");
            Log($"  🆕 New Car Model: {Flags.enableNewCarModel.IsEnabled()}");
        }

        private void Log(string message)
        {
            if (enableDebugLogs) Debug.Log($"[CloudBees] {message}");
        }

        [ContextMenu("Refresh All Flags")] public void RefreshFlags() { if (IsInitialized) LogAllFlags(); }
        [ContextMenu("Force Reinitialize")] public void ForceReinitialize() { IsInitialized = false; InitializeCloudBees(); }
    }
}
