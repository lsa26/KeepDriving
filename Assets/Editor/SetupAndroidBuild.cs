using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class SetupAndroidBuild
{
    public static void Configure()
    {
        Debug.Log("Configuration des paramètres Android...");

        // Changement de plateforme vers Android
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // Paramètres du projet
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.companyName = "Demo";
        PlayerSettings.productName = "CloudBeesDemo";

        // Build system
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        // API graphique
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        GraphicsDeviceType[] devices = { GraphicsDeviceType.OpenGLES3 };
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, devices);

        // Scènes
        string menuScenePath = "Assets/Scenes/Menu.unity"; // Mets ici le vrai chemin si nécessaire
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

        if (System.IO.File.Exists(menuScenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(menuScenePath, true));
        }
        else
        {
            Debug.LogError("Menu.unity introuvable à : " + menuScenePath);
        }

        foreach (var sceneGUID in AssetDatabase.FindAssets("t:scene"))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            if (scenePath != menuScenePath)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
        AssetDatabase.SaveAssets();

        Debug.Log("Configuration Android terminée");
    }
}
