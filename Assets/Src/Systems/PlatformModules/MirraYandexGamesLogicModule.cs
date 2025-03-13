using Cysharp.Threading.Tasks;
using Src.Commands;
using Src.Commands.JsHandle;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Debug;
using Src.Model.Popups;
using UnityEngine;

namespace Src.Systems.PlatformModules
{
    public class MirraYandexGamesLogicModule : PlatformSpecificLogicModuleBase
    {
        private readonly GameStateModel _gameStateModel = GameStateModel.Instance;
        private readonly PlayerModelHolder _playerModelHolder = PlayerModelHolder.Instance;
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        private readonly ScreenCalculator _screenCalculator = ScreenCalculator.Instance;

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
            
            Urls.UpdateBasePathPostfix("/marketYG");
            _playerModelHolder.SetInitialData(playerId, SocialType.YG, isBuyInBankAllowed: true);

            Subscribe();

            await _playerModelHolder.SetUserModelTask;

            if (MirraSdkWrapper.IsAudioPaused)
            {
                UserSettingsModel.SetAudioMutedState(true);
            }
            
            SubscribeAfterPlayerModelLoaded();
        }

        private void Subscribe()
        {
            _dispatcher.SaveCompleted += OnSaveCompleted;
            _dispatcher.UITopPanelRequestOpenLeaderboardsClicked += OnUITopPanelRequestOpenLeaderboardsClicked;
            _dispatcher.UIBankItemClicked += OnUIBankItemClicked;
            _dispatcher.RequestShowAdvert += OnRequestShowAdvert;
            _dispatcher.UILevelUpShareClicked += OnUILevelUpShareClicked;
            _dispatcher.UIOfflineReportShareClicked += OnUIOfflineReportShareClicked;
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
                var expAmount = _playerModelHolder.UserModel.ProgressModel.ExpAmount;
                
                MirraSdkWrapper.SetScore(expAmount);
            }
        }

        private void OnUITopPanelRequestOpenLeaderboardsClicked()
        {
            MirraSdkWrapper.ShowLeaderboard();
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
    }
}