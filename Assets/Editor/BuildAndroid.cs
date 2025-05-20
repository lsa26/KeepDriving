using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

public class BuildAndroid
{
    [MenuItem("Build/Android")]
    public static void Build()
    {
        try 
        {
            Debug.Log("Starting Android build process...");
            
            // Add environment variable detection with fallback
            string buildType = "Development";
            string envBuildType = System.Environment.GetEnvironmentVariable("BUILD_TYPE");
            if (!string.IsNullOrEmpty(envBuildType))
            {
                buildType = envBuildType;
                Debug.Log("Using environment BUILD_TYPE: " + buildType);
            }
            else
            {
                Debug.Log("Environment BUILD_TYPE not found, using default: " + buildType);
            }
            
            // Configuration des scènes
            List<string> enabledScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    enabledScenes.Add(scene.path);
                    Debug.Log("Scene included: " + scene.path);
                }
            }
            
            if (enabledScenes.Count == 0)
            {
                Debug.Log("No active scenes found, looking for all scenes...");
                string[] sceneGuids = AssetDatabase.FindAssets("t:scene");
                foreach (string guid in sceneGuids)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                    enabledScenes.Add(scenePath);
                    Debug.Log("Scene added: " + scenePath);
                }
            }
            
            if (enabledScenes.Count == 0)
            {
                Debug.LogError("No scenes found for build!");
                return;
            }
            
            // Ensure build directory exists
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
                
            string apkPath = Path.Combine(buildPath, "CloudBeesDemo.apk");
            Debug.Log("APK path: " + apkPath);
            
            // Build options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = enabledScenes.ToArray();
            buildPlayerOptions.locationPathName = apkPath;
            buildPlayerOptions.target = BuildTarget.Android;
            
            // Set Android keystore to false
            PlayerSettings.Android.useCustomKeystore = false;
            
            // Set build options based on build type
            if (buildType == "Development")
            {
                buildPlayerOptions.options = BuildOptions.Development | BuildOptions.CompressWithLz4;
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.connectProfiler = false;
            }
            else
            {
                buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.connectProfiler = false;
            }
            
            // Set Android settings
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.bundleVersion = "1.0";
            PlayerSettings.companyName = "Demo";
            PlayerSettings.productName = "CloudBeesDemo";
            
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            
            // Build optimization
            PlayerSettings.bakeCollisionMeshes = true;
            PlayerSettings.stripUnusedMeshComponents = true;
            
            // Start the build
            Debug.Log("Starting Android build...");
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build successful! Time: " + summary.totalTime + " Size: " + summary.totalSize + " bytes");
            }
            else
            {
                Debug.LogError("Build failed: " + summary.result);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during build: " + ex.ToString());
        }
    }
}
