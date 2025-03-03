using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build Asset bundles")]
    static void BuildAll()
    {
        Build("Assets/AssetBundles/WebGL", BuildTarget.WebGL);
        Build("Assets/AssetBundles/OSX", BuildTarget.StandaloneOSX);
        Build("Assets/AssetBundles/Android", BuildTarget.Android);
    }

    private static void Build(string path, BuildTarget target)
    {
        var manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, target);

        Debug.Log($"--------  {target} asset bundle  --------");
        
        foreach (var assetBundleName in manifest.GetAllAssetBundles())
        {
            BuildPipeline.GetCRCForAssetBundle($"{path}/{assetBundleName}", out var crc);
            var hash = manifest.GetAssetBundleHash(assetBundleName);
            
            Debug.Log($"--NAME: " + assetBundleName + " --CRC: " + crc + " --Hash: " + hash);
        }
    }
}
