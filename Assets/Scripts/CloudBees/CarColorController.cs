using UnityEngine;

namespace CloudBees
{
    public class CarColorController : MonoBehaviour
    {
        [Header("Car Parts")]
        [SerializeField] private Renderer[] carParts;
        
        [Header("Available Colors")]
        [SerializeField] private Color redColor = Color.red;
        [SerializeField] private Color blueColor = Color.blue;
        [SerializeField] private Color greenColor = Color.green;
        [SerializeField] private Color yellowColor = Color.yellow;

        private void Start()
        {
            ApplyCarColor();
        }

        private void ApplyCarColor()
        {
            if (CloudBeesManager.Instance == null)
            {
                Debug.LogWarning("⚠️ CloudBeesManager not found, using default red color");
                SetCarColor("red");
                return;
            }

            string selectedColor = CloudBeesManager.Instance.GetCarColor();
            SetCarColor(selectedColor);
        }

        private void SetCarColor(string colorName)
        {
            Color targetColor = GetColorByName(colorName);
            
            foreach (Renderer part in carParts)
            {
                if (part != null)
                {
                    part.material.color = targetColor;
                }
            }
            
            Debug.Log($"🚗 Car color set to: {colorName}");
        }

        private Color GetColorByName(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "red": return redColor;
                case "blue": return blueColor;
                case "green": return greenColor;
                case "yellow": return yellowColor;
                default: 
                    Debug.LogWarning($"⚠️ Unknown color: {colorName}, using red as default");
                    return redColor;
            }
        }

        // Méthode publique pour changer la couleur à la volée (utile pour les tests)
        public void ChangeCarColor(string newColor)
        {
            SetCarColor(newColor);
        }
    }
}
