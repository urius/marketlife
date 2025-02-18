using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Src.Common
{
    public class AssetBundlesLoader
    {
        public static readonly AssetBundlesLoader Instance = new();

        private static string AssetBundlesWebGlUrl => $"{Urls.AssetBundlesBaseUrl}/WebGL";
        private static string AssetBundlesOsxUrl => $"{Urls.AssetBundlesBaseUrl}/OSX";
        private static string AssetBundlesAndroidUrl => $"{Urls.AssetBundlesBaseUrl}/Android";

        private Dictionary<string, AssetBundle> _bundlesByName = new();

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
                PlayerPrefs.SetInt(versionParameterName, version);

                Debug.Log($"Cache for {name} cleared because of new version {version}");
            }

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

                Debug.Log($"Bundle {name} was loaded ");

                return assetBundle;
            }
        }

        private string GetUrl()
        {
            return Application.platform switch
            {
                RuntimePlatform.OSXEditor => AssetBundlesOsxUrl,
                RuntimePlatform.WebGLPlayer => AssetBundlesWebGlUrl,
                RuntimePlatform.Android => AssetBundlesAndroidUrl,
                _ => throw new System.Exception($"AssetBundlesLoader {nameof(GetUrl)}: Unsupported platform type: {Application.platform}"),
            };
        }
    }
}
