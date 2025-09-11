using UnityEngine;
using KeepDriving.CloudBees;

namespace KeepDriving
{
    /// <summary>
    /// Contrôleur principal qui intègre CloudBees avec le gameplay existant
    /// </summary>
    public class GameplayController : MonoBehaviour
    {
        [Header("Gameplay Settings")]
        [SerializeField] private bool waitForCloudBees = true;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject nightModeOverlay;
        
        private bool gameStarted = false;

        private void Start()
        {
            if (waitForCloudBees)
            {
                // Attendre CloudBees avant de démarrer le jeu
                CloudBeesManager.OnFlagsReady += OnCloudBeesReady;
                
                if (CloudBeesManager.AreFeatureFlagsReady())
                {
                    OnCloudBeesReady();
                }
            }
            else
            {
                StartGame();
            }
        }

        private void OnCloudBeesReady()
        {
            Debug.Log("🎮 [Gameplay] CloudBees ready, configuring game...");
            ConfigureGameWithFlags();
            StartGame();
        }

        private void ConfigureGameWithFlags()
        {
            if (!CloudBeesManager.AreFeatureFlagsReady()) return;

            var flags = CloudBeesManager.Flags;

            // 1. Configuration du tutoriel
            bool showTutorial = flags.enableTutorial.IsEnabled();
            Debug.Log($"📚 Tutorial: {(showTutorial ? "ENABLED" : "DISABLED")}");
            
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(showTutorial);
            }

            // 2. Mode nuit
            bool nightMode = flags.enableNightMode.IsEnabled();
            Debug.Log($"🌙 Night Mode: {(nightMode ? "ENABLED" : "DISABLED")}");
            
            if (nightModeOverlay != null)
            {
                nightModeOverlay.SetActive(nightMode);
            }

            // 3. Vitesse maximale (peut être utilisée par votre CarController existant)
            int maxSpeed = flags.maxSpeed.GetValue();
            Debug.Log($"⚡ Max Speed: {maxSpeed} km/h");
            
            // Vous pouvez envoyer cette valeur à votre CarController
            SendMaxSpeedToCarController(maxSpeed);

            // 4. Consommation de carburant
            double fuelMultiplier = flags.fuelConsumption.GetValue();
            Debug.Log($"⛽ Fuel Consumption: {fuelMultiplier}x");
            
            // Vous pouvez ajuster la consommation de carburant
            AdjustFuelConsumption(fuelMultiplier);

            // 5. Nouveau modèle de voiture
            bool newCarModel = flags.enableNewCarModel.IsEnabled();
            Debug.Log($"🚗 New Car Model: {(newCarModel ? "UNLOCKED" : "LOCKED")}");
        }

        private void StartGame()
        {
            if (gameStarted) return;
            
            gameStarted = true;
            Debug.Log("🎮 [Gameplay] Game started!");
            
            // Ici vous pouvez démarrer votre logique de jeu existante
            // Par exemple, activer votre CarController, UI, etc.
        }

        private void SendMaxSpeedToCarController(int maxSpeed)
        {
            // Trouvez votre CarController existant et configurez la vitesse max
            // Exemple :
            // CarController carController = FindObjectOfType<CarController>();
            // if (carController != null)
            // {
            //     carController.SetMaxSpeed(maxSpeed);
            // }
            
            Debug.Log($"🚗 Configuring max speed: {maxSpeed}");
        }

        private void AdjustFuelConsumption(double multiplier)
        {
            // Ajustez la consommation de carburant selon le multiplier
            // Exemple :
            // FuelSystem fuelSystem = FindObjectOfType<FuelSystem>();
            // if (fuelSystem != null)
            // {
            //     fuelSystem.SetConsumptionMultiplier((float)multiplier);
            // }
            
            Debug.Log($"⛽ Adjusting fuel consumption: {multiplier}x");
        }

        private void OnDestroy()
        {
            CloudBeesManager.OnFlagsReady -= OnCloudBeesReady;
        }
    }
}
