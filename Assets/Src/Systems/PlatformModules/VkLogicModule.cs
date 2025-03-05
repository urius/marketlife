using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Commands;
using Src.Commands.JsHandle;
using UnityEngine;

namespace Src.Systems.PlatformModules
{
    public class VkLogicModule : PlatformSpecificLogicModuleBase
    {
        private readonly GameStateModel _gameStateModel = GameStateModel.Instance;
        private readonly PlayerModelHolder _playerModelHolder = PlayerModelHolder.Instance;
        private readonly JsBridge _jsBridge = JsBridge.Instance;
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        private readonly ScreenCalculator _screenCalculator = ScreenCalculator.Instance;
        private readonly SocialUsersData _socialUsersData = SocialUsersData.Instance;
        private readonly NotificationsSystem _notificationsSystem = new();

        private WallPostContext _wallPostContext;
        private UserModel _playerModel;

        public override async void Start()
        {
            Activate();

            await _gameStateModel.GameDataLoadedTask;

            _notificationsSystem.Start();

            ActivateAfterLoad();
        }

        private void Activate()
        {
            _dispatcher.JsIncomingMessage += OnJsIncomingMessage;
            _dispatcher.UIBankItemClicked += OnUIBankItemClicked;
            _dispatcher.UIBottomPanelInviteFriendsClicked += OnUIBottomPanelInviteFriendsClicked;
            _dispatcher.UILevelUpShareClicked += OnUILevelUpShareClicked;
            _dispatcher.UIOfflineReportShareClicked += OnUIOfflineReportShareClicked;
            _dispatcher.RequestShowAdvert += OnRequestShowAdvert;
            _dispatcher.RequestNotifyInactiveFriend += OnRequestNotifyInactiveFriend;
            _dispatcher.UITopPanelRequestOpenLeaderboardsClicked += OnUITopPanelRequestOpenLeaderboardsClicked;

            _socialUsersData.NewUidsRequested += OnSocialUsersDataNewUidsRequested;
        }

        private void ActivateAfterLoad()
        {
            _playerModel = _playerModelHolder.UserModel;
            _playerModel.ProgressModel.LevelChanged += OnLevelChanged;
        }

        private void OnUITopPanelRequestOpenLeaderboardsClicked()
        {
            new ShowInGameLeaderboardsPopupCommand().Execute();
        }

        private void OnSocialUsersDataNewUidsRequested()
        {
            _jsBridge.SendCommandToJs("GetUsersData", new GetUsersDataJsPayload(_socialUsersData.RequestedUids));
        }

        private void OnRequestNotifyInactiveFriend(string friendUid)
        {
            _jsBridge.SendCommandToJs("NotifyInactiveFriend", new NotifyInactiveFriendJsPayload(friendUid));
            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNotifyInactiveFriendRequest);
        }

        private void OnJsIncomingMessage(string message)
        {
            var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
            switch (deserialized.command)
            {
                case "SetVkFriendsData":
                    new SetVkFriendsDataCommand().Execute(message);
                    break;
                case "BuyVkMoneyResult":
                    new ProcessBuyVkMoneyResultCommand().Execute(message);
                    break;
                case "VkWallPostSuccess":
                    ProcessWallPostSuccess();
                    break;
                case "ShowAdsResult":
                    ProcessVkShowAdsResult(message);
                    break;
                case "GetUsersDataResponse":
                    new ProcessVkGetUsersDataCommand().Execute(message);
                    break;
            }
        }

        private async void OnRequestShowAdvert()
        {
            var popupType = _gameStateModel.ShowingPopupModel?.PopupType ?? PopupType.Unknown;
            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventAdsViewClick, ("popup_type", popupType.ToString()));

            _jsBridge.SendCommandToJs("ShowAds", null);

            _dispatcher.UIRequestBlockRaycasts();
            await UniTask.Delay(2000);
            _dispatcher.UIRequestUnblockRaycasts();
        }

        private void ProcessWallPostSuccess()
        {
            _dispatcher.UIShareSuccessCallback();
            if (_wallPostContext != null)
            {
                var screenPoint = _screenCalculator.WorldToScreenPoint(_wallPostContext.ShareButtonWorldPosition);
                _dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, _wallPostContext.rewardAmountGold);
                _playerModel.AddGold(_wallPostContext.rewardAmountGold);
                _wallPostContext = null;
            }
        }

        private void ProcessVkShowAdsResult(string message)
        {
            var deserialized = JsonConvert.DeserializeObject<JsShowAdsResultCommandDto>(message);
            new ProcessShowRewardedAdsResultCommand().Execute(deserialized.data.is_success);
        }

        private void OnUIBottomPanelInviteFriendsClicked()
        {
            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameInviteFriendsClicked);
            _jsBridge.SendCommandToJs("InviteFriends", null);
        }

        private void OnLevelChanged(int delta)
        {
            var newLevel = _playerModel.ProgressModel.Level;
            _jsBridge.SendCommandToJs("LevelUp", new LevelUpJsPayload(newLevel));
        }

        private void OnUIBankItemClicked(BankConfigItem itemConfig)
        {
            _gameStateModel.ChargedBankItem = itemConfig;
            _jsBridge.SendCommandToJs("BuyMoney", new BuyVkMoneyPayload(itemConfig.Id));
        }

        private void OnUILevelUpShareClicked(Vector3 buttonWorldPosition)
        {
            var newLevel = _playerModel.ProgressModel.Level;
            _wallPostContext = new WallPostContext(CalculationHelper.GetLevelUpShareReward(newLevel), buttonWorldPosition);
            _jsBridge.SendCommandToJs("PostNewLevel", new PostNewLevelJsPayload(newLevel));
        }

        private void OnUIOfflineReportShareClicked(Vector3 buttonWorldPosition)
        {
            var offlineReportPopupModel = _gameStateModel.ShowingPopupModel as OfflineReportPopupViewModel;
            var config = GameConfigManager.Instance.MainConfig;
            _wallPostContext = new WallPostContext(config.ShareOfflineReportRewardGold, buttonWorldPosition);
            var payload = new PostOfflineRevenueJsPayload(
                offlineReportPopupModel.ReportModel.CalculationHours,
                offlineReportPopupModel.ReportModel.CalculationMinutes,
                offlineReportPopupModel.ProfitFromSell);
            _jsBridge.SendCommandToJs("PostOfflineRevenue", payload);
        }
        
#region private dtos
        
        private struct PostNewLevelJsPayload
        {
            public int level;

            public PostNewLevelJsPayload(int value)
            {
                level = value;
            }
        }

        private struct BuyVkMoneyPayload
        {
            public string product;

            public BuyVkMoneyPayload(string id)
            {
                product = id;
            }
        }

        private struct GetUsersDataJsPayload
        {
            public string[] uids;

            public GetUsersDataJsPayload(string[] uids)
            {
                this.uids = uids;
            }
        }

        private struct PostOfflineRevenueJsPayload
        {
            public float hours;
            public int minutes;
            public int revenue;

            public PostOfflineRevenueJsPayload(float hoursPassed, int minutesPassed, int value)
            {
                revenue = value;
                hours = hoursPassed;
                minutes = minutesPassed;
            }
        }

        private struct LevelUpJsPayload
        {
            public int level;

            public LevelUpJsPayload(int value)
            {
                level = value;
            }
        }

        private struct NotifyInactiveFriendJsPayload
        {
            public string uid;

            public NotifyInactiveFriendJsPayload(string friendUid)
            {
                uid = friendUid;
            }
        }

        private class WallPostContext
        {
            public readonly Vector3 ShareButtonWorldPosition;
            public readonly int rewardAmountGold;

            public WallPostContext(int rewardGold, Vector3 shareButtonWorldPosition)
            {
                rewardAmountGold = rewardGold;
                ShareButtonWorldPosition = shareButtonWorldPosition;
            }
        }

        private struct JsShowAdsResultCommandDto
        {
            public JsShowAdsResultCommandDataDto data;
        }

        private struct JsShowAdsResultCommandDataDto
        {
            public bool is_success;
        }

#endregion
    }
}