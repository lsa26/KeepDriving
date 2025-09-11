using System;
using UnityEngine;
using System.Threading.Tasks;
// ✅ Décommentons les imports CloudBees
using Io.Rollout.Rox.Core;
using Io.Rollout.Rox.Client.Flags;
using Io.Rollout.Rox.Client;

namespace CloudBees
{
    public class CloudBeesManager : MonoBehaviour
    {
        // ✅ Décommentons le Container CloudBees
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
        private bool _isCloudBeesInitialized = false;

        [Header("CloudBees Configuration")]
        [SerializeField] private bool enableCloudBees = true;
        
        // Variables par défaut pour fallback
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
                // Récupération sécurisée de la clé
                string apiKey = GetSecureApiKey();
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogWarning("⚠️ CloudBees API Key manquante - mode dégradé activé");
                    enableCloudBees = false;
                    return;
                }

                Debug.Log($"🔐 CloudBees
