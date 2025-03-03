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

            var fullUrl = $"{Urls.GetAssetBundleUrl()}/{name}";

            using (var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(fullUrl, (uint)version, 0))
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
                if (webRequestResult.error != null)
                {
                    Debug.Log($"Bundle {name} error: {webRequestResult.error} ");
                }

                var assetBundle = DownloadHandlerAssetBundle.GetContent(webRequestResult);

                _bundlesByName[name] = assetBundle;

                Debug.Log($"Bundle {name} was loaded ");

                return assetBundle;
            }
        }
    }
}
