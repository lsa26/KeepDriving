using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class CacheOptimizer
{
    [MenuItem("Build/Clean Cache")]
    public static void CleanCache()
    {
        Debug.Log("Starting cache cleanup...");
        
        try 
        {
            // Force garbage collection
            GC.Collect();
            
            // Clean unused assets
            Resources.UnloadUnusedAssets();
            
            // Clean asset database cache
            AssetDatabase.Refresh();
            
            Debug.Log("Cache cleanup completed successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during cache cleanup: " + ex.ToString());
        }
    }
}
