using System;
using UnityEngine;
// Commentons temporairement les imports CloudBees pour compiler
// using Io.Rollout.Rox.Core;
// using Io.Rollout.Rox.Client.Flags;

namespace CloudBees
{
    public class CloudBeesManager : MonoBehaviour
    {
        // Commentons temporairement la partie CloudBees pour permettre la compilation
        /*
        public class Container : IRoxContainer
        {
            public RoxString CarColor = new RoxString("red", new[] { "red", "blue", "green", "yellow" });
            
            // Gameplay flags
            public RoxFlag EnablePowerUps = new RoxFlag();
            public RoxInt MaxEnemies = new RoxInt(5);
            public RoxDouble GameSpeed = new RoxDouble(1.0);
            public RoxFlag EnableSpecialEffects = new RoxFlag();
            public RoxFlag EnableBonusRounds = new RoxFlag();
        }

        private Container _container;
        */

        [Header("CloudBees Configuration")]
        [SerializeField] private bool enableCloudBees = true;
        
        // Variables par défaut pour remplacer CloudBees temporairement
        [Header("Default Values (when CloudBees disabled)")]
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

        private void InitializeCloudBees()
        {
            if (!enableCloudBees)
            {
                Debug.Log("🔧 CloudBees désactivé - utilisation des valeurs par défaut");
                return;
            }

            try
            {
                // Récupération sécurisée de la clé
                string apiKey = GetSecureApiKey();
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogWarning("⚠️ CloudBees API Key manquante - mode dégradé activé");
                    enableCloudBees = false;
                    return;
                }

                // TODO: Réactiver quand CloudBees SDK sera correctement installé
                /*
                _container = new Container();
                
                var options = new RoxOptions
                {
                    DevModeSecret = apiKey
                };

                Rox.Setup(apiKey, options).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogError($"❌ CloudBees initialization failed: {task.Exception.Message}");
                        enableCloudBees = false;
                    }
                    else
                    {
                        Debug.Log("✅ CloudBees initialisé avec succès!");
                    }
                });
                */
                
                Debug.Log("🔧 CloudBees en mode simulation");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Erreur CloudBees: {ex.Message}");
                enableCloudBees = false;
            }
        }

        private string GetSecureApiKey()
        {
            try
            {
                TextAsset keyAsset = Resources.Load<TextAsset>("cloudbees_key");
                return keyAsset?.text?.Trim();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Erreur lecture clé CloudBees: {ex.Message}");
                return null;
            }
        }

        // API publique avec fallback
        public string GetCarColor()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.CarColor?.GetValue() ?? defaultCarColor;
            }
            return defaultCarColor;
        }

        public bool IsEnablePowerUps()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.EnablePowerUps?.IsEnabled() ?? defaultEnablePowerUps;
            }
            return defaultEnablePowerUps;
        }

        public int GetMaxEnemies()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.MaxEnemies?.GetValue() ?? defaultMaxEnemies;
            }
            return defaultMaxEnemies;
        }

        public double GetGameSpeed()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.GameSpeed?.GetValue() ?? defaultGameSpeed;
            }
            return defaultGameSpeed;
        }

        public bool IsEnableSpecialEffects()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.EnableSpecialEffects?.IsEnabled() ?? defaultEnableSpecialEffects;
            }
            return defaultEnableSpecialEffects;
        }

        public bool IsEnableBonusRounds()
        {
            if (enableCloudBees)
            {
                // TODO: return _container?.EnableBonusRounds?.IsEnabled() ?? defaultEnableBonusRounds;
            }
            return defaultEnableBonusRounds;
        }
    }
}
