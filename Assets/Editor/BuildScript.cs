using UnityEditor;
using System.IO;
public class BuildAndroid
{
public static void Build()
{
// Configuration Android
SetupAndroidBuild.Configure();
// Configuration des scènes à inclure
string[] scenes = new string[EditorBuildSettings.scenes.Length];
for(int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
scenes[i] = EditorBuildSettings.scenes[i].path;
}
// Création du dossier de build si nécessaire
string buildPath = "Builds";
if (!Directory.Exists(buildPath))
Directory.CreateDirectory(buildPath);
// Lancement de la build
BuildPipeline.BuildPlayer(scenes, 
buildPath + "/CloudBeesDemo.apk", 
BuildTarget.Android, 
BuildOptions.None);
}
}
