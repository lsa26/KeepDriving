using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using KeepDriving.CloudBees; // ✅ Ajout CloudBees

public class GameController : MonoBehaviour {

    public static GameController gameController;

    //... Benchmarking & Save Data Variables
    [Serializable]
    public class UserSettings
    {
        public int quality;  // 0 -low, 1-high
        public int soundVol;  // 100 = 0, 200 = 1vol level
        public int highScore1;
        public bool fps_on; // 0-off, 1-on
        public bool firstRun;  // to decide to show tutorial?
        public bool vibrate; //vibrate or not
        // for future use.
        public int var1;
        public int var2;
        public int var3;
        
        // ✅ Nouvelles variables CloudBees
        public string lastCarColor; // Pour sauvegarder la dernière couleur
        public bool cloudBeesEnabled; // Flag pour activer/désactiver CloudBees
        
        public UserSettings() // default settings
        {
            firstRun = true;
            quality = 0;
            soundVol = 200;  // 100% volume
            highScore1 = 0;
            fps_on = false;
            vibrate = true;
            var1 = 80;  //slider 10-100, existing users 0, new users 100
            var2 = 0;  // avg score
            var3 = 0;  // gamecount for avg score.
            
            // ✅ Valeurs par défaut CloudBees
            lastCarColor = "Blue";
            cloudBeesEnabled = true;
        }
    }
    
    [HideInInspector]public UserSettings userSettings, loadData;
    bool gameLaunch = true;
    public bool gameRun = false; // variable not used currently
    
    // ✅ Variables CloudBees
    [Header("CloudBees Integration")]
    [SerializeField] private bool enableCloudBees = true;
    [SerializeField] private bool debugCloudBees = true;
    
    private bool cloudBeesInitialized = false;

    void Awake()
    {
       //Application.targetFrameRate = 24;
        //.. Singleton design, only one isntance.
        if (gameController == null)
        {
            DontDestroyOnLoad(gameObject);
            gameController = this;
        }
        else if (gameController != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        userSettings = new UserSettings(); //load default values/settings
        Load();  // load save data
        
        // ✅ Initialiser CloudBees après le chargement des paramètres
        if (enableCloudBees && userSettings.cloudBeesEnabled)
        {
            InitializeCloudBees();
        }
        else
        {
            LogCloudBees("CloudBees disabled in settings");
        }
        
        //Application.targetFrameRate = 30;
    }

    // ✅ Nouvelle méthode : Initialisation CloudBees
    private void InitializeCloudBees()
    {
        LogCloudBees("Initializing CloudBees integration...");
        
        if (CloudBeesManager.AreFeatureFlagsReady())
        {
            OnCloudBeesReady();
        }
        else
        {
            CloudBeesManager.OnFlagsReady += OnCloudBeesReady;
        }
        
        // S'abonner aux changements de couleur
        CloudBeesManager.OnCarColorChanged += OnCarColorChanged;
    }

    // ✅ Nouvelle méthode : Callback quand CloudBees est prêt
    private void OnCloudBeesReady()
    {
        cloudBeesInitialized = true;
        LogCloudBees("CloudBees initialized successfully!");
        
        ApplyCloudBeesFlagsToGame();
    }

    // ✅ Nouvelle méthode : Appliquer les flags CloudBees
    private void ApplyCloudBeesFlagsToGame()
    {
        if (!CloudBeesManager.AreFeatureFlagsReady()) return;

        var flags = CloudBeesManager.Flags;
        
        // 1. Gestion du tutorial avec CloudBees
        bool tutorialFromCloudBees = flags.enableTutorial.IsEnabled();
        LogCloudBees($"Tutorial flag: {tutorialFromCloudBees}");
        
        // Override firstRun si CloudBees le demande
        if (tutorialFromCloudBees && !userSettings.firstRun)
        {
            LogCloudBees("CloudBees forcing tutorial display");
            userSettings.firstRun = true; // Force l'affichage du tutorial
        }
        
        // 2. Couleur de voiture
        string carColor = flags.carColor.GetValue();
        LogCloudBees($"Car color from CloudBees: {carColor}");
        
        if (carColor != userSettings.lastCarColor)
        {
            userSettings.lastCarColor = carColor;
            LogCloudBees($"Car color updated to: {carColor}");
        }
        
        // 3. Autres paramètres optionnels
        int maxSpeed = flags.maxSpeed.GetValue();
        LogCloudBees($"Max speed from CloudBees: {maxSpeed} km/h");
        
        // Vous pourriez stocker maxSpeed dans var2 par exemple
        userSettings.var2 = maxSpeed;
        
        // 4. Mode nuit
        bool nightMode = flags.enableNightMode.IsEnabled();
        LogCloudBees($"Night mode: {nightMode}");
        
        // Sauvegarder les changements
        Save();
    }

    // ✅ Nouvelle méthode : Callback changement de couleur
    private void OnCarColorChanged(string newColor)
    {
        LogCloudBees($"Car color changed to: {newColor}");
        userSettings.lastCarColor = newColor;
        Save(); // Sauvegarder immédiatement
    }

    // ✅ Nouvelle méthode : Log CloudBees
    private void LogCloudBees(string message)
    {
        if (debugCloudBees)
        {
            Debug.Log($"[GameController-CloudBees] {message}");
        }
    }

    // ✅ Méthodes publiques pour accéder aux flags CloudBees
    public bool IsCloudBeesReady()
    {
        return cloudBeesInitialized && CloudBeesManager.AreFeatureFlagsReady();
    }

    public string GetCurrentCarColor()
    {
        if (IsCloudBeesReady())
        {
            return CloudBeesManager.Flags.carColor.GetValue();
        }
        return userSettings.lastCarColor;
    }

    public int GetMaxSpeedFromCloudBees()
    {
        if (IsCloudBeesReady())
        {
            return CloudBeesManager.Flags.maxSpeed.GetValue();
        }
        return 120; // Valeur par défaut
    }

    public bool ShouldShowTutorial()
    {
        // Combine la logique existante avec CloudBees
        bool normalFirstRun = userSettings.firstRun;
        
        if (IsCloudBeesReady())
        {
            bool cloudBeesWantsTutorial = CloudBeesManager.Flags.enableTutorial.IsEnabled();
            return normalFirstRun || cloudBeesWantsTutorial;
        }
        
        return normalFirstRun;
    }

    public void Save()
    {
        //.. using binary for safety, so that player can't modify easily.
        BinaryFormatter bf = new BinaryFormatter();

        // .. in windows this path is under user/appdata/LocalLow/...
        //Debug.Log(Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/playerSettings.dat");
        bf.Serialize(file, userSettings);
        file.Close();
        
        LogCloudBees("Settings saved with CloudBees data");
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerSettings.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerSettings.dat", FileMode.Open);             
                loadData = (UserSettings)bf.Deserialize(file);
                file.Close();
                
                userSettings.firstRun = loadData.firstRun;  
                userSettings.soundVol = loadData.soundVol;
                userSettings.quality = loadData.quality;
                userSettings.highScore1 = loadData.highScore1;
                userSettings.fps_on = loadData.fps_on;
                userSettings.vibrate = loadData.vibrate;
                userSettings.var1 = loadData.var1;
                userSettings.var2 = loadData.var2;
                userSettings.var3 = loadData.var3;

                // ✅ Charger les données CloudBees (avec fallback)
                userSettings.lastCarColor = loadData.lastCarColor ?? "Blue";
                userSettings.cloudBeesEnabled = loadData.cloudBeesEnabled;

            //.... Some adjustments for existing playstore installed users when changes where made,remove otherwise.

            if (userSettings.var1 < 10)  //control sensitivity ,before update var1=0 so a fix. now var1 is from 10-100
            {
                userSettings.var1 = 80;
            }

            if(userSettings.soundVol <10) // for old users, new range is 100-200. Old was 0-1
            {
                userSettings.soundVol = 200;
            }
            
            // ✅ Ajustements CloudBees pour les anciens utilisateurs
            if (string.IsNullOrEmpty(userSettings.lastCarColor))
            {
                userSettings.lastCarColor = "Blue";
            }
            
            //.... adjustments over.
        }
        
        LogCloudBees("Settings loaded");
    }

    // Update is called once per frame
    void Update () {

    }

    // ✅ Nettoyage à la destruction
    private void OnDestroy()
    {
        if (CloudBeesManager.Instance != null)
        {
            CloudBeesManager.OnFlagsReady -= OnCloudBeesReady;
            CloudBeesManager.OnCarColorChanged -= OnCarColorChanged;
        }
    }
}
