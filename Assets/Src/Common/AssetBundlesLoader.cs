using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "AssetBundlesLoader", menuName = "Scriptable Objects/Asset Bundles/AssetBundlesLoader")]
public class AssetBundlesLoader : ScriptableObject
{
    public static AssetBundlesLoader Instance { get; private set; }

    [SerializeField] private string _assetBundlesURL;

    private Dictionary<string, AssetBundle> _bundlesByName = new Dictionary<string, AssetBundle>();

    public AssetBundle GetLoadedBundle(string name)
    {
        if (_bundlesByName.TryGetValue(name, out var result))
        {
            return result;
        }

        return null;
    }

    public async UniTask<AssetBundle> LoadOrGetBundle(string name, int version)
    {
        if (_bundlesByName == null)
        {
            _bundlesByName = new Dictionary<string, AssetBundle>();
            Debug.Log("AssetBundlesLoader: LoadOrGetBundle -> recreated _bundlesByName");
        }

        if (_bundlesByName.ContainsKey(name))
        {
            return _bundlesByName[name];
        }

        var fullUrl = $"{_assetBundlesURL}/{name}";
        var versionParameterName = $"bundleversion_{name}";
        var lastSavedVersion = PlayerPrefs.GetInt(versionParameterName);
        if (lastSavedVersion != version)
        {
            Caching.ClearAllCachedVersions(name);
            PlayerPrefs.SetInt(versionParameterName, version);

            Debug.Log($"Cache for {name} cleared because of new version {version}");
        }

        var isFromCache = Caching.IsVersionCached(fullUrl, version);
        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(fullUrl, (uint)version, 0))
        {
            var webRequestResult = await webRequest.SendWebRequest();
            var assetBundle = DownloadHandlerAssetBundle.GetContent(webRequestResult);

            _bundlesByName[name] = assetBundle;

            Debug.Log($"Bundle {name} was loaded " + (isFromCache ? "from cache" : ""));

            return assetBundle;
        }
    }

    private void OnEnable()
    {
        Instance = this;

        _bundlesByName = new Dictionary<string, AssetBundle>();

        _bundlesByName.Clear();
    }
}
