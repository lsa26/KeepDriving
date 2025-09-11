using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildAndroid
{
    [MenuItem("Build/Android APK")]
    public static void Build()
    {
        Debug.Log("🚀 Starting Android build...");
        
        try
        {
            // Configuration du build
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
                Debug.Log($"📁 Created Builds directory: {buildPath}");
            }
            
            string apkPath = Path.Combine(buildPath, "KeepDriving-CloudBees.apk");
            
            // Configuration des paramètres Android
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            
            // Paramètres CloudBees
            string buildType = System.Environment.GetEnvironmentVariable("BUILD_TYPE") ?? "Development";
            bool isDevelopment = buildType == "Development";
            
            Debug.Log($"📋 Build Type: {buildType}");
            Debug.Log($"📋 Development Build: {isDevelopment}");
            
            // Paramètres Unity
            PlayerSettings.productName = "KeepDriving CloudBees";
            PlayerSettings.applicationIdentifier = "com.keepdriving.cloudbees";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            
            // Options de build
            BuildOptions buildOptions = BuildOptions.None;
            if (isDevelopment)
            {
                buildOptions |= BuildOptions.Development;
                buildOptions |= BuildOptions.AllowDebugging;
                Debug.Log("🔧 Development build options enabled");
            }
            
            // Scènes à inclure
            string[] scenes = GetScenesFromBuildSettings();
            
            Debug.Log($"📋 Scenes to build: {scenes.Length}");
            foreach (string scene in scenes)
            {
                Debug.Log($"  - {scene}");
            }
            
            // Lancement du build
            Debug.Log("🔨 Starting build process...");
            BuildReport report = BuildPipeline.BuildPlayer(
                scenes,
                apkPath,
                BuildTarget.Android,
                buildOptions
            );
            
            // Vérification du résultat
            if (report.summary.result == BuildResult.Succeeded)
            {
                long size = report.summary.totalSize;
                Debug.Log($"✅ Build succeeded! APK size: {size / (1024 * 1024)} MB");
                Debug.Log($"📦 APK location: {apkPath}");
                Debug.Log($"⏱️ Build time: {report.summary.totalTime.TotalSeconds:F1} seconds");
            }
            else
            {
                Debug.LogError($"❌ Build failed with result: {report.summary.result}");
                
                // Afficher les erreurs détaillées
                foreach (BuildStep step in report.steps)
                {
                    foreach (BuildStepMessage message in step.messages)
                    {
                        if (message.type == LogType.Error || message.type == LogType.Exception)
                        {
                            Debug.LogError($"Build Error: {message.content}");
                        }
                    }
                }
                
                EditorApplication.Exit(1);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Build exception: {ex.Message}");
            Debug.LogError($"Stack trace: {ex.StackTrace}");
            EditorApplication.Exit(1);
        }
    }
    
    private static string[] GetScenesFromBuildSettings()
    {
        EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
        
        if (buildSettingsScenes.Length == 0)
        {
            Debug.LogWarning("⚠️ No scenes found in Build Settings. Adding current scene.");
            // Si aucune scène n'est configurée, essayer d'ajouter la scène actuelle
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(currentScene.path))
            {
                return new string[] { currentScene.path };
            }
            else
            {
                Debug.LogError("❌ No valid scenes found for build!");
                return new string[0];
            }
        }
        
        string[] scenes = new string[buildSettingsScenes.Length];
        for (int i = 0; i < buildSettingsScenes.Length; i++)
        {
            scenes[i] = buildSettingsScenes[i].path;
        }
        
        return scenes;
    }
}
