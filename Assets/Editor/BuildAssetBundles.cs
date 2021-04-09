using UnityEditor;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build Asset bundles")]
    static void BuildAll()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }
}
