using System;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;
using romanlee17.MirraGames;
using romanlee17.MirraGames.Interfaces;
using UnityEngine;

namespace Src.Common
{
    public class MirraSdkWrapper
    {
        public static readonly MirraSdkWrapper Instance = new();
        
        private static UniTaskCompletionSource<string> _getPlayerIdTcs;

        public static bool IsYandexGames => MirraSDK.Platform.Current == PlatformType.Web_YandexGames;
        public static bool IsVk => MirraSDK.Platform.Current == PlatformType.Web_VKontakte;
        
        [DllImport("__Internal")]
        private static extern void GetYGPlayerId(Action<string> callback);

        
        public static UniTask<string> GetPlayerId()
        {
            return Instance.GetPlayerIdInternal();
        }

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

        private UniTask<string> GetPlayerIdInternal()
        {
            _getPlayerIdTcs = new UniTaskCompletionSource<string>();

            if (IsYandexGames)
            {
                GetYGPlayerId(GetYGPlayerIdCallback);
            }
            else
            {
                _getPlayerIdTcs.TrySetResult(MirraSDK.Player.PlatformToken);
            }
            
            return _getPlayerIdTcs.Task;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void GetYGPlayerIdCallback(string playerId)
        {
            _getPlayerIdTcs.TrySetResult(playerId);
        }
    }
}