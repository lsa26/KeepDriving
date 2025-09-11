using UnityEngine;
using UnityEditor;
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
            
            // Options de build
            BuildOptions buildOptions = BuildOptions.None;
            if (isDevelopment)
            {
                buildOptions |= BuildOptions.Development;
                buildOptions |= BuildOptions.AllowDebugging;
            }
            
            // Scènes à inclure
            string[] scenes = GetScenesFromBuildSettings();
            
            Debug.Log($"📋 Scenes to build: {scenes.Length}");
            foreach (string scene in scenes)
            {
                Debug.Log($"  - {scene}");
            }
            
            // Lancement du build
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
            }
            else
            {
                Debug.LogError($"❌ Build failed with result: {report.summary.result}");
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
        string[] scenes = new string[buildSettingsScenes.Length];
        
        for (int i = 0; i < buildSettingsScenes.Length; i++)
        {
            scenes[i] = buildSettingsScenes[i].path;
        }
        
        return scenes;
    }
}
