using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "AssetBundlesLoader", menuName = "Scriptable Objects/Asset Bundles/AssetBundlesLoader")]
public class AssetBundlesLoader : ScriptableObject
{
    public static AssetBundlesLoader Instance { get; private set; }

    [SerializeField] private string _assetBundlesWebGLURL;
    [SerializeField] private string _assetBundlesOSXURL;

    private Dictionary<string, AssetBundle> _bundlesByName = new Dictionary<string, AssetBundle>();

    public AssetBundle GetLoadedBundle(string name)
    {
        if (_bundlesByName.TryGetValue(name, out var result))
        {
            return result;
        }

        return null;
    }

    public async UniTask<AssetBundle> LoadOrGetBundle(string name, int version = -1, Action<float> progressCallback = null)
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

        var fullUrl = $"{GetUrl()}/{name}";
        var versionParameterName = $"bundleversion_{name}";
        var lastSavedVersion = PlayerPrefs.GetInt(versionParameterName);
        if (lastSavedVersion != version || version < 0)
        {
            Caching.ClearAllCachedVersions(name);
            PlayerPrefs.SetInt(versionParameterName, version);

            Debug.Log($"Cache for {name} cleared because of new version {version}");
        }

        var isFromCache = Caching.IsVersionCached(fullUrl, version);
        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(fullUrl, (uint)version, 0))
        {
            var sendRequestOperation = webRequest.SendWebRequest();
            if (progressCallback != null)
            {
                while (!sendRequestOperation.isDone)
                {
                    await UniTask.Delay(100);
                    progressCallback(sendRequestOperation.progress);
                }
            }
            var webRequestResult = await sendRequestOperation;
            var assetBundle = DownloadHandlerAssetBundle.GetContent(webRequestResult);

            _bundlesByName[name] = assetBundle;

            Debug.Log($"Bundle {name} was loaded " + (isFromCache ? "from cache" : ""));

            return assetBundle;
        }
    }

    private string GetUrl()
    {
        return Application.platform switch
        {
            RuntimePlatform.OSXEditor => _assetBundlesOSXURL,
            RuntimePlatform.WebGLPlayer => _assetBundlesWebGLURL,
            _ => throw new System.Exception($"AssetBundlesLoader {nameof(GetUrl)}: Unsupported platform type: {Application.platform}"),
        };
    }

    private void OnEnable()
    {
        Instance = this;

        _bundlesByName = new Dictionary<string, AssetBundle>();

        _bundlesByName.Clear();
    }
}
