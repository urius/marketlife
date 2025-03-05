using Cysharp.Threading.Tasks;
using Src.Commands.JsHandle;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;

namespace Src.Systems.PlatformModules
{
    public class MirraYandexGamesLogicModule : PlatformSpecificLogicModuleBase
    {
        private readonly GameStateModel _gameStateModel = GameStateModel.Instance;
        private readonly PlayerModelHolder _playerModelHolder = PlayerModelHolder.Instance;
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;

        public override void Start()
        {
            StartInternal().Forget();
        }

        private async UniTaskVoid StartInternal()
        {
            var playerId = await MirraSdkWrapper.GetPlayerId();
            
            Urls.UpdateBasePathPostfix("/marketYG");
            _playerModelHolder.SetInitialData(playerId, SocialType.YG);

            Subscribe();
        }

        private void Subscribe()
        {
            _dispatcher.SaveCompleted += OnSaveCompleted;
            _dispatcher.UITopPanelRequestOpenLeaderboardsClicked += OnUITopPanelRequestOpenLeaderboardsClicked;
            _dispatcher.RequestShowAdvert += OnRequestShowAdvert;
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
    }
}