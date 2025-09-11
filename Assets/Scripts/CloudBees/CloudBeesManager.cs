using UnityEngine;
using System.Threading.Tasks;
using System;
using Io.Rollout.Rox.Client;
using Io.Rollout.Rox.Core.Entities;
using Io.Rollout.Rox.Client.Flags;

namespace KeepDriving.CloudBees
{
    /// <summary>
    /// Conteneur des feature flags pour KeepDriving
    /// </summary>
    public class CarFlags : IRoxContainer 
    {
        public RoxString carColor = new RoxString("Blue", new string[] 
        {
            "Blue", "Red", "Green", "Yellow", "White", "Black", "Purple", "Orange"
        });
        
        public RoxFlag enableTutorial = new RoxFlag(false);
        public RoxInt maxSpeed = new RoxInt(120, new int[] {80, 100, 120, 150, 200});
        public RoxDouble fuelConsumption = new RoxDouble(1.0, new double[] {0.5, 0.8, 1.0, 1.2, 1.5});
        public RoxFlag enableNightMode = new RoxFlag(false);
        public RoxFlag enableNewCarModel = new RoxFlag(false);
    }

    /// <summary>
    /// Manager principal pour CloudBees Feature Flags avec sécurité renforcée
    /// </summary>
    public class CloudBeesManager : MonoBehaviour
    {
        [Header("CloudBees Configuration")]
        [SerializeField] private string fallbackSdkKey = ""; // ⚠️ Laissez vide pour forcer l'usage de Resources
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool forceOfflineMode = false;
        
        // Propriétés statiques
        public static CarFlags Flags { get; private set; }
        public static bool IsInitialized { get; private set; } = false;
        public static CloudBeesManager Instance { get; private set; }

        // Events
        public static event System.Action OnFlagsReady;
        public static event System.Action<string> OnCarColorChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (!forceOfflineMode)
            {
                InitializeCloudBees();
            }
            else
            {
                InitializeOfflineMode();
            }
        }

        private async void InitializeCloudBees()
        {
            try
            {
                Log("🚀 Initializing CloudBees Feature Flags...");
                
                // 🔐 Récupérer la SDK key de manière sécurisée
                string sdkKey = LoadSecureSDKKey();
                
                if (string.IsNullOrEmpty(sdkKey))
                {
                    Log("❌ No SDK key found, switching to offline mode");
                    InitializeOfflineMode();
                    return;
                }
                
                // Créer le conteneur de flags
                Flags = new CarFlags();
                
                // Enregistrer le conteneur
                Rox.Register("", Flags);
                
                // Configuration options
                var options = new RoxOptions(new RoxOptions.RoxOptionsBuilder {});
                
                // Setup avec la clé SDK sécurisée
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

        /// <summary>
        /// 🔐 Charge la SDK key de manière sécurisée depuis Resources
        /// </summary>
        private string LoadSecureSDKKey()
        {
            try
            {
                // 1. Essayer de charger depuis Resources (Jenkins)
                TextAsset keyAsset = Resources.Load<TextAsset>("cloudbees_key");
                if (keyAsset != null && !string.IsNullOrEmpty(keyAsset.text))
                {
                    Log("🔐 SDK key loaded securely from Resources");
                    return keyAsset.text.Trim();
                }
                
                // 2. Fallback vers la clé configurée dans l'Inspector (développement local)
                if (!string.IsNullOrEmpty(fallbackSdkKey))
                {
                    Log("🔧 Using fallback SDK key from Inspector (dev mode)");
                    return fallbackSdkKey;
                }
                
                // 3. Aucune clé trouvée
                Log("⚠️ No SDK key found in Resources or fallback");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error loading SDK key: {ex.Message}");
                return null;
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
            if (enableDebugLogs)
            {
                Debug.Log($"[CloudBees] {message}");
            }
        }

        // Méthodes utilitaires publiques
        public static bool AreFeatureFlagsReady()
        {
            return IsInitialized && Flags != null;
        }

        public static string GetCarColor()
        {
            return AreFeatureFlagsReady() ? Flags.carColor.GetValue() : "Blue";
        }

        public static void NotifyCarColorChanged(string newColor)
        {
            OnCarColorChanged?.Invoke(newColor);
        }

        [ContextMenu("Refresh All Flags")]
        public void RefreshFlags()
        {
            if (IsInitialized)
            {
                LogAllFlags();
            }
        }

        [ContextMenu("Force Reinitialize")]
        public void ForceReinitialize()
        {
            IsInitialized = false;
            InitializeCloudBees();
        }
    }
}
