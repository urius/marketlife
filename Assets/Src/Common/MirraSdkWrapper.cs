using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;
using romanlee17.MirraGames;
using romanlee17.MirraGames.Interfaces;
using Src.Model.Debug;
using UnityEngine;

namespace Src.Common
{
    public class MirraSdkWrapper
    {
        public static readonly MirraSdkWrapper Instance = new();
        
        private static UniTaskCompletionSource<string> _getPlayerIdTcs;

        public static bool IsYandexGames => MirraSDK.Platform.Current == PlatformType.Web_YandexGames;
        public static bool IsVk => MirraSDK.Platform.Current == PlatformType.Web_VKontakte;
        public static bool IsMirraSdkUsed => IsVk == false;

        public static LanguageType CurrentLanguage => MirraSDK.Language.Current;
        public static bool IsRussianLanguage => CurrentLanguage == LanguageType.Russian;
        public static bool IsEnglishLanguage => CurrentLanguage == LanguageType.English;

        public static float AudioVolume
        {
            get => MirraSDK.Audio.Volume;
            set => MirraSDK.Audio.Volume = value;
        }

        public static bool IsAudioPaused
        {
            get => MirraSDK.Audio.Pause;
            set => MirraSDK.Audio.Pause = value;
        }

        [DllImport("__Internal")]
        private static extern void GetYGPlayerId(Action<string> callback);


        public static void SendGameReady()
        {
            MirraSDK.Analytics.GameIsReady();
        }
        
        public static UniTask<string> GetPlayerId()
        {
            return Instance.GetPlayerIdInternal();
        }

        public static void SetScore(int scoreValue, string scoreTag = "score")
        {
            Log("SetScore: " + scoreValue);
            
            MirraSDK.Socials.SetScore(scoreTag, scoreValue);
        }

        public static void ShowLeaderboard(string scoreTag = "score")
        {
            Log("Request ShowLeaderboard, scoreTag: " + scoreTag);
            
            MirraSDK.Socials.InvokeLeaderboard(scoreTag);
        }

        public static bool IsRewardedReady()
        {
            return MirraSDK.Ads.IsRewardedReady;
        }

        public static UniTask<bool> ShowRewardedAd()
        {
            var tcs = new UniTaskCompletionSource<bool>();
            
            MirraSDK.Ads.InvokeRewarded(
                onSuccess: () =>
                {
                    tcs.TrySetResult(true);
                },
                onNotReady: () =>
                {
                    tcs.TrySetResult(false);
                },
                onAnyClose: () =>
                {
                    tcs.TrySetResult(false);
                },
                rewardTag: "dynamic"
            );
            
            return tcs.Task;
        }

        public static UniTask<BankProductData[]> FetchProducts()
        {
            var tcs = new UniTaskCompletionSource<BankProductData[]>();

#if UNITY_EDITOR
            tcs.TrySetResult(new[]
            {
                new BankProductData("cash_500", 100, "TestCurrency"),
                new BankProductData("gold_100", 500, "TestCurrencyGold"),
            });
#else
            MirraSDK.Payments.Fetch(d =>
            {
                if (d.Products != null)
                {
                    var result = d.Products
                        .Select(p => new BankProductData(p.Tag, p.PriceInteger, p.Currency))
                        .ToArray();
                    
                    tcs.TrySetResult(result);
                }

                tcs.TrySetResult(Array.Empty<BankProductData>());
            });
#endif
            return tcs.Task;
        }

        public static UniTask<bool> Purchase(string productTag)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            MirraSDK.Payments.Purchase(productTag,
                onSuccess: () =>
                {
                    tcs.TrySetResult(true);
                },
                onError: () =>
                {
                    Log($"Purchase failed: {productTag}");
                    tcs.TrySetResult(false);
                });
            
            return tcs.Task;
        }

        public static void Consume(string productTag)
        {
            MirraSDK.Payments.Fetch(d =>
            {
                d.ConsumeProduct(productTag, () =>
                {
                    Log($"Consume: Consumed product {productTag}");
                });
            });
        }

        public static void FetchAndConsume(Action<string> consumeAction)
        {
            MirraSDK.Payments.Fetch(d =>
            {
                if (d.Products == null) return;
                
                foreach (var productData in d.Products)
                {
                    var tag = productData.Tag;
                    d.ConsumeProduct(productData.Tag, () =>
                    {
                        Log($"FetchAndConsume: Consumed product {tag}");
                        consumeAction?.Invoke(tag);
                    });
                }
            });
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

#if UNITY_EDITOR
            _getPlayerIdTcs.TrySetResult(DebugDataHolder.Instance.DebugUid);
#else         
            if (IsYandexGames)
            {
                GetYGPlayerId(GetYGPlayerIdCallback);
            }
            else
            {
                _getPlayerIdTcs.TrySetResult(MirraSDK.Player.PlatformToken);
            }
#endif
            return _getPlayerIdTcs.Task;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void GetYGPlayerIdCallback(string playerId)
        {
            _getPlayerIdTcs.TrySetResult(playerId);
        }
    }

    public struct BankProductData
    {
        public string ProductId;
        public int Price;
        public string CurrencyNameLocalized;

        public BankProductData(string id, int price, string currencyNameLocalized)
        {
            ProductId = id;
            Price = price;
            CurrencyNameLocalized = currencyNameLocalized;
        }
    }
}