using System;
using UnityEngine;
using System.Threading.Tasks;
// ✅ Namespaces pour rox-client (les vraies DLLs téléchargées)
using Io.Rollout.Rox.Core;
using Io.Rollout.Rox.Client.Flags;
using Io.Rollout.Rox.Client;

namespace CloudBees
{
    public class CloudBeesManager : MonoBehaviour
    {
        // ✅ Container avec les DLLs rox-client téléchargées
        public class Container : IRoxContainer
        {
            // ✅ Syntaxe rox-client (pas rox-server)
            public RoxString CarColor = new RoxString("red", new string[] { "red", "blue", "green", "yellow" });
            public RoxFlag EnablePowerUps = new RoxFlag();
            public RoxInt MaxEnemies = new RoxInt(5);
            public RoxDouble GameSpeed = new RoxDouble(1.0);
            public RoxFlag EnableSpecialEffects = new RoxFlag();
            public RoxFlag EnableBonusRounds = new RoxFlag();
        }

        private Container _container;
        private bool _isCloudBeesInitialized = false;

        [Header("CloudBees Configuration")]
        [SerializeField] private bool enableCloudBees = true;
        
        [Header("Default Values (fallback)")]
        public string defaultCarColor = "red";
        public bool defaultEnablePowerUps = true;
        public int defaultMaxEnemies = 5;
        public double defaultGameSpeed = 1.0;
        public bool defaultEnableSpecialEffects = true;
        public bool defaultEnableBonusRounds = false;

        private static CloudBeesManager _instance;
        public static CloudBeesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CloudBeesManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CloudBeesManager");
                        _instance = go.AddComponent<CloudBeesManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCloudBees();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private async void InitializeCloudBees()
        {
            if (!enableCloudBees)
            {
                Debug.Log("🔧 CloudBees désactivé - utilisation des valeurs par défaut");
                return;
            }

            try
            {
                string apiKey = GetSecureApiKey();
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogWarning("⚠️ CloudBees API Key manquante - mode dégradé activé");
                    enableCloudBees = false;
                    return;
                }

                Debug.Log($"🔐 CloudBees: Initializing with key: {apiKey.Substring(0, Math.Min(8, apiKey.Length))}...");

                // ✅ Initialisation CloudBees avec rox-client
                _container = new Container();
                
                // ✅ Register le container
                await Rox.Register(_container);
                
                // ✅ Options pour rox-client
                var options = new RoxOptions.Builder().Build();

                // ✅ Setup avec votre clé API
                await Rox.Setup(apiKey, options);
                
                _isCloudBeesInitialized = true;
                Debug.Log("✅ CloudBees (rox-client) initialisé avec succès!");
                
                LogCurrentFeatureFlags();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Erreur CloudBees: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                enableCloudBees = false;
            }
        }

        private void LogCurrentFeatureFlags()
        {
            if (!_isCloudBeesInitialized || _container == null) return;
            
            try
            {
                Debug.Log("🎯 Current CloudBees Feature Flags (rox-client):");
                Debug.Log($"  - Car Color: {_container.CarColor.GetValue()}");
                Debug.Log($"  - Power-ups: {_container.EnablePowerUps.IsEnabled()}");
                Debug.Log($"  - Max Enemies: {_container.MaxEnemies.GetValue()}");
                Debug.Log($"  - Game Speed: {_container.GameSpeed.GetValue()}");
                Debug.Log($"  - Special Effects: {_container.EnableSpecialEffects.IsEnabled()}");
                Debug.Log($"  - Bonus Rounds: {_container.EnableBonusRounds.IsEnabled()}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Error logging feature flags: {ex.Message}");
            }
        }

        private string GetSecureApiKey()
        {
            try
            {
                TextAsset keyAsset = Resources.Load<TextAsset>("cloudbees_key");
                string key = keyAsset?.text?.Trim();
                
                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogError("❌ CloudBees key is empty or null");
                    return null;
                }
                
                Debug.Log($"✅ CloudBees key loaded successfully (length: {key.Length})");
                return key;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Erreur lecture clé CloudBees: {ex.Message}");
                return null;
            }
        }

        // ✅ API publique avec vraies feature flags CloudBees rox-client
        public string GetCarColor()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    string color = _container.CarColor.GetValue();
                    Debug.Log($"🚗 CloudBees Car Color: {color}");
                    return color;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees CarColor error: {ex.Message}");
                }
            }
            return defaultCarColor;
        }

        public bool IsEnablePowerUps()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    bool enabled = _container.EnablePowerUps.IsEnabled();
                    Debug.Log($"⚡ CloudBees Power-ups: {enabled}");
                    return enabled;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees EnablePowerUps error: {ex.Message}");
                }
            }
            return defaultEnablePowerUps;
        }

        public int GetMaxEnemies()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    int maxEnemies = _container.MaxEnemies.GetValue();
                    Debug.Log($"👹 CloudBees Max Enemies: {maxEnemies}");
                    return maxEnemies;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees MaxEnemies error: {ex.Message}");
                }
            }
            return defaultMaxEnemies;
        }

        public double GetGameSpeed()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    double speed = _container.GameSpeed.GetValue();
                    Debug.Log($"🏃 CloudBees Game Speed: {speed}");
                    return speed;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees GameSpeed error: {ex.Message}");
                }
            }
            return defaultGameSpeed;
        }

        public bool IsEnableSpecialEffects()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    bool enabled = _container.EnableSpecialEffects.IsEnabled();
                    Debug.Log($"✨ CloudBees Special Effects: {enabled}");
                    return enabled;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees EnableSpecialEffects error: {ex.Message}");
                }
            }
            return defaultEnableSpecialEffects;
        }

        public bool IsEnableBonusRounds()
        {
            if (_isCloudBeesInitialized && _container != null)
            {
                try
                {
                    bool enabled = _container.EnableBonusRounds.IsEnabled();
                    Debug.Log($"🎁 CloudBees Bonus Rounds: {enabled}");
                    return enabled;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ CloudBees EnableBonusRounds error: {ex.Message}");
                }
            }
            return defaultEnableBonusRounds;
        }

        public bool IsCloudBeesConnected()
        {
            return _isCloudBeesInitialized && _container != null;
        }
    }
}
