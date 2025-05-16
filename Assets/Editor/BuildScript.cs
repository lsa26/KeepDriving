using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build.Reporting; // Ajout pour BuildReport, BuildSummary et BuildResult

public class BuildScript
{
    [MenuItem("Build/Android")]
    public static void Build()
    {
        try {
            Debug.Log("Configuration de la build pour Android...");
            
            // Appel à la configuration
            SetupAndroidBuild.Configure();
            
            // Configuration des scènes
            List<string> enabledScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    enabledScenes.Add(scene.path);
                    Debug.Log("Scene incluse: " + scene.path);
                }
            }
            
            // Si aucune scène n'est activée, chercher toutes les scènes disponibles
            if (enabledScenes.Count == 0)
            {
                Debug.Log("Aucune scène active trouvée, recherche de toutes les scènes...");
                foreach (var sceneGUID in AssetDatabase.FindAssets("t:scene"))
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                    enabledScenes.Add(scenePath);
                    Debug.Log("Scene ajoutée: " + scenePath);
                }
            }
            
            // Vérification des scènes
            if (enabledScenes.Count == 0)
            {
                Debug.LogError("Aucune scène trouvée pour la build!");
                return;
            }
            
            // Création du dossier de build
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);
                
            string apkPath = Path.Combine(buildPath, "CloudBeesDemo.apk");
            Debug.Log("Chemin de l'APK: " + apkPath);
            
            // Options de build
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = enabledScenes.ToArray();
            buildPlayerOptions.locationPathName = apkPath;
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;
            
            // Lancement de la build
            Debug.Log("Démarrage de la build Android...");
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build réussie! Temps: " + summary.totalTime + " Taille: " + summary.totalSize + " bytes");
            }
            else
            {
                Debug.LogError("La build a échoué: " + summary.result);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur lors de la build: " + ex.ToString());
        }
    }
}
