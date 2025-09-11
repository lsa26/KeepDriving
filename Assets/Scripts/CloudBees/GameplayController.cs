using UnityEngine;

namespace CloudBees
{
    public class GameplayController : MonoBehaviour
    {
        [Header("Gameplay Settings")]
        [SerializeField] private GameObject[] powerUpPrefabs;
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private ParticleSystem[] specialEffects;
        
        [Header("Game Speed")]
        [SerializeField] private float baseGameSpeed = 1.0f;
        
        private bool powerUpsEnabled;
        private int maxEnemies;
        private float currentGameSpeed;
        private bool specialEffectsEnabled;
        private bool bonusRoundsEnabled;
        
        private int currentEnemyCount = 0;

        private void Start()
        {
            InitializeGameplay();
        }

        private void InitializeGameplay()
        {
            if (CloudBeesManager.Instance == null)
            {
                Debug.LogWarning("⚠️ CloudBeesManager not found, using default gameplay settings");
                SetDefaultSettings();
                return;
            }

            // Récupérer les paramètres CloudBees
            powerUpsEnabled = CloudBeesManager.Instance.IsEnablePowerUps();
            maxEnemies = CloudBeesManager.Instance.GetMaxEnemies();
            currentGameSpeed = (float)CloudBeesManager.Instance.GetGameSpeed();
            specialEffectsEnabled = CloudBeesManager.Instance.IsEnableSpecialEffects();
            bonusRoundsEnabled = CloudBeesManager.Instance.IsEnableBonusRounds();

            ApplyGameplaySettings();
        }

        private void SetDefaultSettings()
        {
            powerUpsEnabled = true;
            maxEnemies = 5;
            currentGameSpeed = 1.0f;
            specialEffectsEnabled = true;
            bonusRoundsEnabled = false;
            
            ApplyGameplaySettings();
        }

        private void ApplyGameplaySettings()
        {
            // Appliquer la vitesse de jeu
            Time.timeScale = currentGameSpeed * baseGameSpeed;
            
            // Activer/désactiver les power-ups
            foreach (GameObject powerUp in powerUpPrefabs)
            {
                if (powerUp != null)
                {
                    powerUp.SetActive(powerUpsEnabled);
                }
            }
            
            // Activer/désactiver les effets spéciaux
            foreach (ParticleSystem effect in specialEffects)
            {
                if (effect != null)
                {
                    if (specialEffectsEnabled)
                        effect.Play();
                    else
                        effect.Stop();
                }
            }

            Debug.Log($"🎮 Gameplay Settings Applied:");
            Debug.Log($"  - Power-ups: {powerUpsEnabled}");
            Debug.Log($"  - Max Enemies: {maxEnemies}");
            Debug.Log($"  - Game Speed: {currentGameSpeed}");
            Debug.Log($"  - Special Effects: {specialEffectsEnabled}");
            Debug.Log($"  - Bonus Rounds: {bonusRoundsEnabled}");
        }

        // Méthodes publiques pour le contrôle du gameplay
        public bool CanSpawnEnemy()
        {
            return currentEnemyCount < maxEnemies;
        }

        public void OnEnemySpawned()
        {
            currentEnemyCount++;
        }

        public void OnEnemyDestroyed()
        {
            currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        }

        public bool ArePowerUpsEnabled()
        {
            return powerUpsEnabled;
        }

        public bool AreSpecialEffectsEnabled()
        {
            return specialEffectsEnabled;
        }

        public bool AreBonusRoundsEnabled()
        {
            return bonusRoundsEnabled;
        }

        public float GetCurrentGameSpeed()
        {
            return currentGameSpeed;
        }

        // Méthode pour rafraîchir les paramètres (utile si CloudBees est mis à jour en temps réel)
        public void RefreshSettings()
        {
            InitializeGameplay();
        }
    }
}
