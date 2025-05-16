using UnityEditor;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        string[] scenes = FindEnabledEditorScenes();
        
        // Configurer les paramètres de build pour Android
        PlayerSettings.Android.keystorePass = "android";
        PlayerSettings.Android.keyaliasPass = "android";
        
        // Définir le chemin de sortie pour l'APK
        string buildPath = "build";
        if (!Directory.Exists(buildPath))
            Directory.CreateDirectory(buildPath);
            
        string apkPath = Path.Combine(buildPath, "KeepDriving.apk");
        
        // Lancer la build Android
        BuildPipeline.BuildPlayer(scenes, apkPath, BuildTarget.Android, BuildOptions.None);
    }
    
    private static string[] FindEnabledEditorScenes()
    {
        // Récupère toutes les scènes actives du projet
        var editorScenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                editorScenes.Add(scene.path);
        }
        return editorScenes.ToArray();
    }
}
