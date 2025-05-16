using UnityEditor;
using System.IO;
using System.Collections.Generic;
public class SetupAndroidBuild
{
public static void Configure()
{
// Changement de plateforme vers Android
EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
// Configuration des paramètres Android
PlayerSettings.Android.bundleVersionCode = 1;
PlayerSettings.bundleVersion = "1.0";
PlayerSettings.companyName = "Demo";
PlayerSettings.productName = "CloudBeesDemo";
// Activer le autographics API pour Android
PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
List<GraphicsDeviceType> devices = new List<GraphicsDeviceType>();
devices.Add(GraphicsDeviceType.OpenGLES3);
PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, devices.ToArray());
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
// Log pour confirmer la configuration
Debug.Log("Android build configuration completed");
}
}
