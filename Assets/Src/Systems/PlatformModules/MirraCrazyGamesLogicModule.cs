using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;
using Src.Commands;
using Src.Commands.JsHandle;
using Src.Common;
using Src.Common_Utils;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Debug;
using Src.Model.Popups;
using Src.Net;
using UnityEngine;

namespace Src.Systems.PlatformModules
{
    public class MirraCrazyGamesLogicModule : PlatformSpecificLogicModuleBase
    {
        private const string LastUserInfoSaveTsKey = "last_user_info_save_ts";
        private const int SecondsInWeek = 60*60*24*7;
        
        [DllImport("__Internal")]
        private static extern void GetCGUser(Action<string, string> callback);
        
        private readonly GameStateModel _gameStateModel = GameStateModel.Instance;
        private readonly PlayerModelHolder _playerModelHolder = PlayerModelHolder.Instance;
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        private readonly ScreenCalculator _screenCalculator = ScreenCalculator.Instance;
        private readonly SocialUsersData _socialUsersData = SocialUsersData.Instance;
        
        private static UniTaskCompletionSource<GetUserInfoData> _cgGetUserInfoTcs;

        private UserSettingsModel UserSettingsModel => _playerModelHolder.UserModel.UserSettingsModel;
        
        
        public override void Start()
        {
            StartInternal().Forget();
        }

        private async UniTaskVoid StartInternal()
        {
            var playerId = await MirraSdkWrapper.GetPlayerId();
            
#if UNITY_EDITOR
            playerId = DebugDataHolder.Instance.DebugUid;
#endif
            
            Urls.UpdateBasePathPostfix("/marketCrazy");
            _playerModelHolder.SetInitialData(playerId, SocialType.CrazyGames, isBuyInBankAllowed: true);

            Subscribe();

            await _playerModelHolder.SetUserModelTask;

            if (MirraSdkWrapper.IsAudioPaused)
            {
                UserSettingsModel.SetAudioMutedState(true);
            }
            
            SubscribeAfterPlayerModelLoaded();

            SaveUserInfoIfNeeded().Forget();
        }

        private void Subscribe()
        {
            Debug.Log("MirraCrazyGamesLogicModule Subscribe");
            
            _dispatcher.SaveCompleted += OnSaveCompleted;
            _dispatcher.UITopPanelRequestOpenLeaderboardsClicked += OnUITopPanelRequestOpenLeaderboardsClicked;
            _dispatcher.UIBankItemClicked += OnUIBankItemClicked;
            _dispatcher.RequestShowAdvert += OnRequestShowAdvert;
            _dispatcher.UILevelUpShareClicked += OnUILevelUpShareClicked;
            _dispatcher.UIOfflineReportShareClicked += OnUIOfflineReportShareClicked;
            _gameStateModel.PopupRemoved += OnPopupRemoved;
            
            _socialUsersData.NewUidsRequested += OnSocialUsersDataNewUidsRequested;
        }

        private void SubscribeAfterPlayerModelLoaded()
        {
            UserSettingsModel.AudioMutedStateChanged += OnAudioMutedStateChanged;
        }

        private void OnAudioMutedStateChanged()
        {
            MirraSdkWrapper.IsAudioPaused = UserSettingsModel.IsAudioMuted;
        }

        private void OnUIBankItemClicked(BankConfigItem itemConfig)
        {
            ProcessBuyBankItem(itemConfig).Forget();
        }

        private async UniTaskVoid ProcessBuyBankItem(BankConfigItem itemConfig)
        {
            _gameStateModel.ChargedBankItem = itemConfig;

            var buyResult = await MirraSdkWrapper.Purchase(itemConfig.Id);
            
            new ProcessBuyChargedBankItemResultCommand().Execute(buyResult);
        }

        private void OnSaveCompleted(bool result, SaveField saveField)
        {
            if (result && (saveField & SaveField.Progress) == SaveField.Progress)
            {
                var progressModel = _playerModelHolder.UserModel.ProgressModel;
                
                MirraSdkWrapper.SetScore(Constants.ScoreTagExp, progressModel.ExpAmount);
                //MirraSdkWrapper.SetScore(Constants.ScoreTagCash, progressModel.Cash);
                //MirraSdkWrapper.SetScore(Constants.ScoreTagGold, progressModel.Gold);
            }
        }

        private void OnUITopPanelRequestOpenLeaderboardsClicked()
        {
            new ShowInGameLeaderboardsPopupCommand().Execute();
        }

        private void OnRequestShowAdvert()
        {
            ProcessShowRewardedAds().Forget();
        }

        private async UniTaskVoid ProcessShowRewardedAds()
        {
            var popupType = _gameStateModel.ShowingPopupModel?.PopupType ?? PopupType.Unknown;
            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventAdsViewClick, ("popup_type", popupType.ToString()));

            _dispatcher.UIRequestBlockRaycasts();

            var showAdsResult = await MirraSdkWrapper.ShowRewardedAd();
            
            _dispatcher.UIRequestUnblockRaycasts();

            new ProcessShowRewardedAdsResultCommand().Execute(showAdsResult);
        }

        private void OnUILevelUpShareClicked(Vector3 buttonWorldPosition)
        {
            var newLevel = _playerModelHolder.UserModel.ProgressModel.Level;
            var reward = CalculationHelper.GetLevelUpShareReward(newLevel);
            
            MirraSdkWrapper.ShareThisGame();

            ProcessWallPostSuccess(reward, buttonWorldPosition).Forget();
        }

        private void OnUIOfflineReportShareClicked(Vector3 buttonWorldPosition)
        {
            var config = GameConfigManager.Instance.MainConfig;

            var reward = config.ShareOfflineReportRewardGold;

            MirraSdkWrapper.ShareThisGame();

            ProcessWallPostSuccess(reward, buttonWorldPosition).Forget();
        }

        private async UniTaskVoid ProcessWallPostSuccess(int rewardAmountGold, Vector3 shareButtonWorldPosition)
        {
            _dispatcher.UIShareSuccessCallback();

            await UniTask.Delay(500);

            var screenPoint = _screenCalculator.WorldToScreenPoint(shareButtonWorldPosition);
            _dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, rewardAmountGold);
            _playerModelHolder.UserModel.AddGold(rewardAmountGold);
        }

        private async void OnSocialUsersDataNewUidsRequested()
        {
            var uidsJoined = string.Join(',', _socialUsersData.RequestedUids);

            Debug.Log("OnSocialUsersDataNewUidsRequested, uids count = " + _socialUsersData.RequestedUids.Length);

            var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(Urls.GetUsersInfoURL, uidsJoined);

            if (resultOperation.IsSuccess)
            {
                var userItemsDto = JsonUtility.FromJson<GetUsersInfoResponseDto>(resultOperation.Result.response);

                var socialDataList = userItemsDto.users.Select(ToUserSocialData).ToArray();

                foreach (var socialData in socialDataList)
                {
                    AvatarsManager.Instance.SetupAvatarSettings(socialData.Uid, socialData.Picture50Url);
                }

                _socialUsersData.FillRequestedSocialData(socialDataList);

                Debug.Log("OnSocialUsersDataNewUidsRequested, socialDataList = " + socialDataList.Length);
            }
        }

        private static UserSocialData ToUserSocialData(GetUsersInfoItemDto dto)
        {
            return new UserSocialData(dto.id, dto.name, string.Empty, dto.picture_url);
        }

        private async UniTaskVoid SaveUserInfoIfNeeded()
        {
            var lastUserInfoSaveTs = PlayerPrefs.GetInt(LastUserInfoSaveTsKey, 0);
            var currentTs = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            
            if (currentTs - lastUserInfoSaveTs > SecondsInWeek)
            {
                Debug.Log("ProcessSetUserInfo");
                
                var userInfo = await GetCrazyGamesUserInfo();

                var uid = _playerModelHolder.UserModel.Uid;

                if (string.IsNullOrEmpty(uid)) return;
                if (string.IsNullOrEmpty(userInfo.Name)) return;
                
                var url = string.Format(Urls.SetUserInfoURL, uid, userInfo.Name, userInfo.PictureUrl);
                
                var resultOperation = await new WebRequestsSender().GetAsync(url);
                
                Debug.Log("ProcessSetUserInfo completed, result: " + resultOperation.Result);

                if (resultOperation.IsSuccess)
                {
                    PlayerPrefs.SetInt(LastUserInfoSaveTsKey, currentTs);
                }
            }
            else
            {
                Debug.Log("SetUserInfo skipped");
            }
        }

        private UniTask<GetUserInfoData> GetCrazyGamesUserInfo()
        {
            _cgGetUserInfoTcs = new UniTaskCompletionSource<GetUserInfoData>();

#if UNITY_EDITOR
            _cgGetUserInfoTcs.TrySetResult(
                new GetUserInfoData { Name = "test Name", PictureUrl = "https://imgs.crazygames.com/userportal/avatars/89.png" });
#else
            GetCGUser(GetCGPlayerInfoCallback);
#endif

            return _cgGetUserInfoTcs.Task;
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void GetCGPlayerInfoCallback(string playerName, string playerPictureUrl)
        {
            Debug.Log(" GetCGPlayerInfoCallback: " + playerName + " url:" + playerPictureUrl);

            _cgGetUserInfoTcs.TrySetResult(new GetUserInfoData()
            {
                Name = playerName,
                PictureUrl = playerPictureUrl,
            });
        }

        private void OnPopupRemoved(PopupViewModelBase popupViewModel)
        {
            if (popupViewModel.PopupType == PopupType.LevelUp)
            {
                MirraSdkWrapper.ShowInterstitialAd().Forget();
            }
        }

        private struct GetUserInfoData
        {
            public string Name;
            public string PictureUrl;
        }
        
        [Serializable]
        private class GetUsersInfoResponseDto
        {
            public GetUsersInfoItemDto[] users;
        }
        
        [Serializable]
        private class GetUsersInfoItemDto
        {
            public string id;
            public string name;
            public string picture_url;
        }
    }
}