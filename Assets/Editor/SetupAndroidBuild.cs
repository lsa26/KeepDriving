using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
        
        // Configuration des scènes
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (var sceneGUID in AssetDatabase.FindAssets("t:scene"))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
        
        // Sauvegarde des paramètres
        AssetDatabase.SaveAssets();
        
        Debug.Log("Configuration Android terminée");
    }
}
