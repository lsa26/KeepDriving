using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking; // Ajouté pour UnityWebRequest

// Conditional compilation pour les vrais namespaces
#if CLOUDBEES_SERVER_AVAILABLE
using Io.Rollout.Rox.Server;
using Io.Rollout.Rox.Core.Entities;
using Io.Rollout.Rox.Server.Flags;
#endif

namespace CloudBees
{
    public class CloudBeesManager : MonoBehaviour
    {
        // Classes de fallback si rox-server n'est pas disponible
#if !CLOUDBEES_SERVER_AVAILABLE
        public interface IRoxContainer { }
        
        public class RoxFlag
        {
            private bool _default;
            
            public RoxFlag(bool defaultValue = false)
            {
                _default = defaultValue;
            }
            
            public bool IsEnabled()
            {
                // Simulation réaliste avec pattern temporel
                return ((int)(Time.time / 5f) % 3) != 0; // 66% true
            }
        }
        
        public class RoxString
        {
            private string _default;
            private string[] _options;
            
            public RoxString(string defaultValue, string[] options)
            {
                _default = defaultValue;
                _options = options ?? new[] { defaultValue };
            }
            
            public string GetValue()
            {
                // Simulation avec rotation basée sur le temps
                int index = (int)(Time.time / 10f) % _options.Length;
                return _options[index];
            }
        }
        
        public class RoxInt
        {
            private int _default;
            private int[] _options;
            
            public RoxInt(int defaultValue, int[] options = null)
            {
                _default = defaultValue;
                _options = options ?? new[] { defaultValue };
            }
            
            public int GetValue()
            {
                int index = (int)(Time.time / 8f) % _options.Length;
                return _options[index];
            }
        }
        
        public class RoxDouble
        {
            private double _default;
            private double[] _options;
            
            public RoxDouble(double defaultValue, double[] options = null)
            {
                _default = defaultValue;
                _options = options ?? new[] { defaultValue };
            }
            
            public double GetValue()
            {
                int index = (int)(Time.time / 12f) % _options.Length;
                return _options[index];
            }
        }
        
        public static class Rox
        {
            public static Task Register(string name, object container)
            {
                Debug.Log($"📦 CloudBees: Container '{name}' registered (simulation mode)");
                return Task.CompletedTask;
            }
            
            public static Task Setup(string apiKey, object options = null)
            {
                Debug.Log($"🔐 CloudBees: Setup with key {apiKey.Substring(0, 8)}... (simulation mode)");
                return Task.CompletedTask;
            }
        }
        
        public class RoxOptions
        {
            public class RoxOptionsBuilder { }
            
            public RoxOptions(RoxOptionsBuilder builder)
            {
                // Constructor for simulation
            }
        }
#endif

        // ✅ Container avec la VRAIE syntaxe officielle CloudBees
        public class Flags : IRoxContainer
        {
            // ✅ Syntaxe exacte selon la documentation
            public RoxFlag enableTutorial = new RoxFlag(false);
            public RoxString CarColor = new RoxString("red", new string[] { "red", "blue", "green", "yellow" });
            public RoxInt MaxEnemies = new RoxInt(5, new int[] { 3, 5, 7, 10 });
            public RoxDouble GameSpeed = new RoxDouble(1.0, new double[] { 0.8, 1.0, 1.2, 1.5 });
            public RoxFlag EnablePowerUps = new RoxFlag(true);
            public RoxFlag EnableSpecialEffects = new RoxFlag(true);
            public RoxFlag EnableBonusRounds = new RoxFlag(false);
        }

        private Flags _flags;
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

            // DIAGNOSTIC: Vérifie le symbole de compilation et les assemblies
            #if CLOUDBEES_SERVER_AVAILABLE
                Debug.Log("✅ DIRECTIVE CLOUDBEES_SERVER_AVAILABLE ACTIVE - Mode réel");
                // Tester si les DLLs sont correctement chargées
                try {
                    var assemblyTest = typeof(Io.Rollout.Rox.Server.RoxOptions);
                    Debug.Log($"✅ CloudBees SDK trouvé: {assemblyTest.Assembly.FullName}");
                } catch (Exception e) {
                    Debug.LogError($"❌ CloudBees SDK introuvable malgré la directive: {e.Message}");
                }
            #else
                Debug.LogError("❌ DIRECTIVE CLOUDBEES_SERVER_AVAILABLE NON ACTIVE - Mode simulation");
            #endif

            // DIAGNOSTIC: Test de connectivité réseau
            StartCoroutine(TestConnectivity());

            try
            {
                // Recherche du fichier de clé avec diagnostic amélioré
                string apiKey = GetSecureApiKey();
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogWarning("⚠️ CloudBees API Key manquante - mode dégradé activé");
                    enableCloudBees = false;
                    return;
                }

                Debug.Log($"🔐 CloudBees rox-server: Initializing with key: {apiKey.Substring(0, Math.Min(8, apiKey.Length))}...");

                // ✅ Initialisation avec la VRAIE syntaxe officielle
                _flags = new Flags();
                
                // ✅ Register avec namespace vide comme dans la doc
                await Rox.Register("", _flags);
                
                // ✅ Options avec la vraie syntaxe
                var options = new RoxOptions(new RoxOptions.RoxOptionsBuilder { });

                // ✅ Setup avec votre clé
                await Rox.Setup(apiKey, options);
                
                _isCloudBeesInitialized = true;
                
#if CLOUDBEES_SERVER_AVAILABLE
                Debug.Log("✅ CloudBees rox-server REAL SDK initialized successfully!");
#else
                Debug.Log("✅ CloudBees rox-server SIMULATION initialized successfully!");
#endif
                
                LogCurrentFeatureFlags();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Erreur CloudBees rox-server: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                enableCloudBees = false;
            }
        }

        // DIAGNOSTIC: Test de connectivité réseau
        private IEnumerator TestConnectivity() 
        {
            Debug.Log("🔍 Test de connectivité CloudBees en cours...");
            
            using (UnityWebRequest www = UnityWebRequest.Get("https://api.cloudbees.io/v1/status")) 
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success) 
                {
                    Debug.Log("✅ Connectivité Internet OK: " + www.downloadHandler.text);
                } 
                else 
                {
                    Debug.LogError($"❌ Problème de connectivité: {www.error}");
                }
            }
        }

        private void LogCurrentFeatureFlags()
        {
            if (!_isCloudBeesInitialized || _flags == null) return;
            
            try
            {
#if CLOUDBEES_SERVER_AVAILABLE
                Debug.Log("🎯 Current Feature Flags (rox-server REAL):");
#else
                Debug.Log("🎯 Current Feature Flags (rox-server SIMULATION):");
#endif
                Debug.Log($"  - Tutorial: {_flags.enableTutorial.IsEnabled()}");
                Debug.Log($"  - Car Color: {_flags.CarColor.GetValue()}");
                Debug.Log($"  - Power-ups: {_flags.EnablePowerUps.IsEnabled()}");
                Debug.Log($"  - Max Enemies: {_flags.MaxEnemies.GetValue()}");
                Debug.Log($"  - Game Speed: {_flags.GameSpeed.GetValue():F2}");
                Debug.Log($"  - Special Effects: {_flags.EnableSpecialEffects.IsEnabled()}");
                Debug.Log($"  - Bonus Rounds: {_flags.EnableBonusRounds.IsEnabled()}");
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
                // Essaie d'abord sans extension (comme avant)
                TextAsset keyAsset = Resources.Load<TextAsset>("cloudbees_key");
                
                // Si null, essaie avec extension .txt (comme créé par Jenkins)
                if (keyAsset == null)
                {
                    Debug.Log("📝 'cloudbees_key' non trouvé, essai avec 'cloudbees_key.txt'...");
                    keyAsset = Resources.Load<TextAsset>("cloudbees_key.txt");
                }
                
                string key = keyAsset?.text?.Trim();
                
                if (string.IsNullOrEmpty(key))
                {
                    // Liste tous les fichiers dans Resources pour aider au diagnostic
                    Debug.LogError("❌ CloudBees key is empty or null, vérification des ressources disponibles...");
                    var resourcesInfo = Resources.LoadAll("");
                    Debug.Log($"📁 {resourcesInfo.Length} ressources trouvées dans le dossier Resources:");
                    foreach (var res in resourcesInfo)
                    {
                        Debug.Log($"   - {res.name} ({res.GetType().Name})");
                    }
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

        // API publique utilisant la vraie syntaxe rox-server
        public string GetCarColor()
        {
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    string color = _flags.CarColor.GetValue();
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
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    bool enabled = _flags.EnablePowerUps.IsEnabled();
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
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    int maxEnemies = _flags.MaxEnemies.GetValue();
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
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    double speed = _flags.GameSpeed.GetValue();
                    Debug.Log($"🏃 CloudBees Game Speed: {speed:F2}");
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
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    bool enabled = _flags.EnableSpecialEffects.IsEnabled();
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
            if (_isCloudBeesInitialized && _flags != null)
            {
                try
                {
                    bool enabled = _flags.EnableBonusRounds.IsEnabled();
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
            return _isCloudBeesInitialized && _flags != null;
        }

        public void RefreshFeatureFlags()
        {
            if (_isCloudBeesInitialized)
            {
                LogCurrentFeatureFlags();
            }
        }
    }
}
