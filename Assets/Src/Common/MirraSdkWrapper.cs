using Cysharp.Threading.Tasks;
using romanlee17.MirraGames;
using romanlee17.MirraGames.Interfaces;
using UnityEngine;

namespace Src.Common
{
    public class MirraSdkWrapper
    {
        public static readonly MirraSdkWrapper Instance = new();

        public static string PlayerId => MirraSDK.Player.PlatformToken;
        public static bool IsYandexGames => MirraSDK.Platform.Current == PlatformType.Web_YandexGames;

        public static UniTask<AssetBundle> LoadAssetBundle(string bundleTag, string bundleUrl)
        {
            return Instance.LoadAssetBundleInternal(bundleTag, bundleUrl);
        }
        
        public static void Log(string message)
        {
            Debug.Log($"Mirra {message}");
        }
        
        private UniTask<AssetBundle> LoadAssetBundleInternal(string bundleTag, string bundleUrl)
        {
            var tcs = new UniTaskCompletionSource<AssetBundle>();

            MirraSDK.Remote.ResolveBundle(bundleTag, bundleUrl,
                b => tcs.TrySetResult(b),
                () => { tcs.TrySetResult(null); });

            return tcs.Task;
        }
    }
}