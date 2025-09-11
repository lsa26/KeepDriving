using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KeepDriving.CloudBees;

namespace KeepDriving
{
    /// <summary>
    /// Contrôleur pour changer la couleur de la voiture via CloudBees
    /// </summary>
    public class CarColorController : MonoBehaviour
    {
        [Header("Car Configuration")]
        [SerializeField] private Renderer[] carRenderers;
        [SerializeField] private bool autoFindRenderers = true;
        
        [Header("Color Palette")]
        [SerializeField] private Color blueColor = new Color(0.2f, 0.4f, 1f);
        [SerializeField] private Color redColor = new Color(1f, 0.2f, 0.2f);
        [SerializeField] private Color greenColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color yellowColor = new Color(1f, 1f, 0.2f);
        [SerializeField] private Color whiteColor = Color.white;
        [SerializeField] private Color blackColor = new Color(0.1f, 0.1f, 0.1f);
        [SerializeField] private Color purpleColor = new Color(0.8f, 0.2f, 0.8f);
        [SerializeField] private Color orangeColor = new Color(1f, 0.6f, 0.2f);

        private Dictionary<string, Color> colorMap;
        private string currentColor = "Blue";

        private void Awake()
        {
            InitializeColorMap();
            
            if (autoFindRenderers && (carRenderers == null || carRenderers.Length == 0))
            {
                FindCarRenderers();
            }
        }

        private void Start()
        {
            // S'abonner aux événements CloudBees
            CloudBeesManager.OnFlagsReady += OnCloudBeesFlagsReady;
            CloudBeesManager.OnCarColorChanged += OnCarColorChanged;
            
            // Attendre CloudBees si pas encore prêt
            if (CloudBeesManager.AreFeatureFlagsReady())
            {
                ApplyCarColorFromFlags();
            }
            else
            {
                StartCoroutine(WaitForCloudBeesAndApplyColor());
            }
        }

        private void InitializeColorMap()
        {
            colorMap = new Dictionary<string, Color>
            {
                {"Blue", blueColor},
                {"Red", redColor},
                {"Green", greenColor},
                {"Yellow", yellowColor},
                {"White", whiteColor},
                {"Black", blackColor},
                {"Purple", purpleColor},
                {"Orange", orangeColor}
            };
        }

        private void FindCarRenderers()
        {
            // Auto-trouver tous les renderers sur cet objet et ses enfants
            List<Renderer> renderers = new List<Renderer>();
            
            // Renderer sur cet objet
            Renderer selfRenderer = GetComponent<Renderer>();
            if (selfRenderer != null)
            {
                renderers.Add(selfRenderer);
            }
            
            // Renderers sur les enfants
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            renderers.AddRange(childRenderers);
            
            carRenderers = renderers.ToArray();
            Debug.Log($"[CarColor] Auto-found {carRenderers.Length} renderers");
        }

        private IEnumerator WaitForCloudBeesAndApplyColor()
        {
            Debug.Log("⏳ [CarColor] Waiting for CloudBees initialization...");
            
            while (!CloudBeesManager.AreFeatureFlagsReady())
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            ApplyCarColorFromFlags();
        }

        private void OnCloudBeesFlagsReady()
        {
            ApplyCarColorFromFlags();
        }

        private void OnCarColorChanged(string newColor)
        {
            if (newColor != currentColor)
            {
                ApplyCarColor(newColor);
            }
        }

        private void ApplyCarColorFromFlags()
        {
            if (!CloudBeesManager.AreFeatureFlagsReady()) return;

            string selectedColor = CloudBeesManager.GetCarColor();
            ApplyCarColor(selectedColor);
        }

        private void ApplyCarColor(string colorName)
        {
            Debug.Log($"🎨 [CarColor] Changing car color to: {colorName}");

            if (!colorMap.ContainsKey(colorName))
            {
                Debug.LogWarning($"[CarColor] Color '{colorName}' not found, using Blue as default");
                colorName = "Blue";
            }

            Color targetColor = colorMap[colorName];
            currentColor = colorName;
            
            // Appliquer la couleur à tous les renderers
            foreach (var renderer in carRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = targetColor;
                }
            }

            Debug.Log($"✅ [CarColor] Car color successfully changed to: {colorName}");
            CloudBeesManager.NotifyCarColorChanged(colorName);
        }

        // Méthodes pour les tests manuels
        [ContextMenu("Apply Color from CloudBees")]
        public void ManuallyApplyColorFromFlags()
        {
            ApplyCarColorFromFlags();
        }

        [ContextMenu("Test Red Color")]
        public void TestRedColor()
        {
            ApplyCarColor("Red");
        }

        [ContextMenu("Test Blue Color")]
        public void TestBlueColor()
        {
            ApplyCarColor("Blue");
        }

        private void OnDestroy()
        {
            // Se désabonner des événements
            CloudBeesManager.OnFlagsReady -= OnCloudBeesFlagsReady;
            CloudBeesManager.OnCarColorChanged -= OnCarColorChanged;
        }

        private void OnValidate()
        {
            // Auto-setup dans l'editor
            if (autoFindRenderers && Application.isEditor)
            {
                FindCarRenderers();
            }
        }
    }
}
