using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    public static void Build()
    {
        Debug.Log("Démarrage de la build Android...");

        // ✖️ Désactiver toute configuration de keystore personnalisée
        PlayerSettings.Android.useCustomKeystore = false;

        string[] scenes = { "Assets/Scenes/Main.unity" };
        string buildPath = "Builds/CloudBeesDemo.apk";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✅ Build réussie!");
            Debug.Log("📦 Chemin de l'APK: " + buildPath);
        }
        else
        {
            Debug.LogError("❌ La build a échoué: " + summary.result);
        }
    }
}
