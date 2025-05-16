using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering; // Ajout pour GraphicsDeviceType

public class SetupAndroidBuild
{
    public static void Configure()
    {
        Debug.Log("Configuration des paramètres Android...");
        
        // Changement de plateforme vers Android
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        
        // Configuration des paramètres Android
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.companyName = "Demo";
        PlayerSettings.productName = "CloudBeesDemo";
        
        // Configuration du build system
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        
        // Configuration de l'API graphique
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        GraphicsDeviceType[] devices = { GraphicsDeviceType.OpenGLES3 };
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, devices);
        
        // Configuration des scènes avec tri pour prioriser la scène principale
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        string mainScenePath = null;
        List<EditorBuildSettingsScene> otherScenes = new List<EditorBuildSettingsScene>();
        
        foreach (var sceneGUID in AssetDatabase.FindAssets("t:scene"))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            var scene = new EditorBuildSettingsScene(scenePath, true);
        
            // Cherche une scène avec un nom typique de scène principale
            if (scenePath.ToLower().Contains("main") || scenePath.ToLower().Contains("menu"))
            {
                if (mainScenePath == null)  // Prend la première qui correspond
                {
                    mainScenePath = scenePath;
                    scenes.Insert(0, scene); // Met en premier
                    continue;
                }
            }
        
            otherScenes.Add(scene);
        }
        
        // Ajouter les autres scènes après
        scenes.AddRange(otherScenes);
        
        // Appliquer à la configuration du build
        EditorBuildSettings.scenes = scenes.ToArray();

        
        // Sauvegarde des paramètres
        AssetDatabase.SaveAssets();
        
        Debug.Log("Configuration Android terminée");
    }
}
