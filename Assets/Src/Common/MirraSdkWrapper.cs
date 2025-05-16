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
        public static bool IsCrazyGames => MirraSDK.Platform.Current == PlatformType.Web_CrazyGames;
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

        public static bool IsAuthorized()
        {
            return MirraSDK.Player.IsAuthorized;
        }

        public static UniTask<bool> InvokeAuthorization()
        {
            var tcs = new UniTaskCompletionSource<bool>();

            MirraSDK.Player.InvokeAuthorization(
                onSuccess: () => { tcs.TrySetResult(true); },
                onError: () => { tcs.TrySetResult(false); });
            
            return tcs.Task;
        }

        public static void SendGameReady()
        {
            MirraSDK.Analytics.GameIsReady();
        }

        public static void SendGameplayStart()
        {
            MirraSDK.Analytics.GameplayStart();
        }

        public static void SendGameplayStop()
        {
            MirraSDK.Analytics.GameplayStop();
        }
        
        public static UniTask<string> GetPlayerId()
        {
            return Instance.GetPlayerIdInternal();
        }

        public static void SetScore(string scoreTag, int scoreValue)
        {
            Log("SetScore: " + scoreTag + " = " + scoreValue);
            
            MirraSDK.Socials.SetScore(scoreTag, scoreValue);
        }

        public static void ShowLeaderboard(string scoreTag = "score")
        {
            Log("Request ShowLeaderboard, scoreTag: " + scoreTag);
            
            MirraSDK.Socials.InvokeLeaderboard(scoreTag);
        }
        
        public static UniTask<MirraLeaderboardUserData[]> GetScoreTable(string scoreTag)
        {
            var tcs = new UniTaskCompletionSource<MirraLeaderboardUserData[]>();
            
            MirraSDK.Socials.GetScoreTable(
                scoreTag: scoreTag, 
                leadingPlayers: 3, 
                includePlayer: true,
                playersAround: 2, 
                onScoreTableResolve: (scoreTable) =>
                {
                    var result = new MirraLeaderboardUserData[scoreTable.Count];
                    for (var i = 0; i < scoreTable.Count; i++)
                    {
                        var scoreTableData = scoreTable[i];
                        result[i] = new MirraLeaderboardUserData(
                            scoreTableData.position, scoreTableData.name, scoreTableData.score, scoreTableData.pictureURL);
                        
                        Log("GetScoreTable value: " + scoreTableData.score);
                    }

                    Log("GetScoreTable success " + scoreTag + " " + result.Length);
                    
                    tcs.TrySetResult(result);
                },
                onScoreTableError: () =>
                {
                    Log("GetScoreTable error" + scoreTag);
                    
                    tcs.TrySetResult(Array.Empty<MirraLeaderboardUserData>());
                }
            );

#if UNITY_EDITOR
            
                var result = new MirraLeaderboardUserData(1, "Name", 100500,
                    "https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium");
                return UniTask.FromResult(new[] { result });
#endif
            
            return tcs.Task;
        }

        public static bool IsRewardedReady()
        {
            return MirraSDK.Ads.IsRewardedReady;
        }

        public static async UniTask<bool> ShowRewardedAd()
        {
            SendGameplayStop();
            
            var tcs = new UniTaskCompletionSource<bool>();
            
            MirraSDK.Ads.InvokeRewarded(
                onSuccess: () =>
                {
                    Log("Mirra ShowRewardedAd onSuccess");
                    tcs.TrySetResult(true);
                },
                onNotReady: () =>
                {
                    Log("Mirra ShowRewardedAd onNotReady");
                    tcs.TrySetResult(false);
                },
                onAnyClose: () =>
                {
                    Log("Mirra ShowRewardedAd onAnyClose");
                    UniTask.DelayFrame(10).ContinueWith(() => tcs.TrySetResult(false));
                },
                rewardTag: "dynamic"
            );
            
            var result = await tcs.Task;

            SendGameplayStart();
            
            return result;
        }

        public static async UniTask ShowInterstitialAd()
        {
            var tcs = new UniTaskCompletionSource();

            SendGameplayStop();
            
            MirraSDK.Ads.InvokeInterstitial(
                ignoreOnce: false,
                onAnyClose: () => { tcs.TrySetResult(); }
            );

            await tcs.Task;
            
            SendGameplayStart();
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

        public static int GetCurrentTimestampSec()
        {
            var currentDateTime = MirraSDK.Time.CurrentDate.ToLocalTime();
            var timeSpan = currentDateTime - DateTime.UnixEpoch.ToLocalTime();
            
            return (int)timeSpan.TotalSeconds;
        }

        public static void SaveString(string keyName, string value)
        {
            MirraSDK.Prefs.SetString(keyName, value);
            MirraSDK.Prefs.Save();

            Log("Save to platform");
        }
        
        public static void DeleteKey(string keyName)
        {
            MirraSDK.Prefs.DeleteKey(keyName);
            MirraSDK.Prefs.Save();
        }

        public static string GetString(string keyName)
        {
            return MirraSDK.Prefs.GetString(keyName);
        }

        public static void ShareThisGame()
        {
            MirraSDK.Socials.ShareThisGame();
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
                Log("MirraSDK.Player.PlatformId: " + MirraSDK.Player.PlatformId);
                
                _getPlayerIdTcs.TrySetResult(MirraSDK.Player.PlatformId);
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

    public struct MirraLeaderboardUserData
    {
        public readonly int Rank;
        public readonly string PlayerName;
        public readonly int Score;
        public readonly string PictureURL;

        public MirraLeaderboardUserData(int rank, string playerName, int score, string pictureURL)
        {
            Rank = rank;
            PlayerName = playerName;
            Score = score;
            PictureURL = pictureURL;
        }
    }
}