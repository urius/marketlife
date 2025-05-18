using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using UnityEngine;

public class PlatformSpecificLogicSystem
{
    private readonly Dispatcher _dispatcher;
    //
    private PlatformSpecificLogicModuleBase _module;

    public PlatformSpecificLogicSystem()
    {
        _dispatcher = Dispatcher.Instance;
    }

    public void Start()
    {
        Activate();
    }

    private void Activate()
    {
        _dispatcher.JsIncomingMessage += OnJsIncomingMessage;
#if UNITY_EDITOR
        InitDebugEditorModule();
#endif
    }

    private void OnJsIncomingMessage(string message)
    {
        var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
        Debug.Log($"Incoming Js command {message}");
        switch (deserialized.command)
        {
            case "SetVkPlatformData":
                InitModuleVK();
                new SetVkPlatformDataCommand().Execute(message);
                break;
        }
    }

    private void InitDebugEditorModule()
    {
        _module = new DebugEditorLogicModule();
        _module.Start();
    }

    private void InitModuleVK()
    {
        _module = new VKLogicModule();
        _module.Start();
    }
}

public abstract class PlatformSpecificLogicModuleBase
{
    public abstract void Start();
}

public class DebugEditorLogicModule : PlatformSpecificLogicModuleBase
{
    private readonly SocialUsersData _socialUsersData;
    private readonly AvatarsManager _avatarsManager;
    private readonly MockDataProvider _mockDataProvider;

    public DebugEditorLogicModule()
    {
        _socialUsersData = SocialUsersData.Instance;
        _avatarsManager = AvatarsManager.Instance;
        _mockDataProvider = MockDataProvider.Instance;
    }

    public override void Start()
    {
        Activate();
    }

    private void Activate()
    {
        _socialUsersData.NewUidsRequested += OnNewUidsRequested;
    }

    private void OnNewUidsRequested()
    {        
        var result = new List<UserSocialData>();
        foreach (var uid in _socialUsersData.RequestedUids)
        {
            _avatarsManager.SetupAvatarSettings(uid, _mockDataProvider.GetMockAvatarUrl());
            result.Add(new UserSocialData(uid, $"-{uid}-", "Lastname", null));
        }
        _socialUsersData.FillRequestedSocialData(result);
    }
}

public class VKLogicModule : PlatformSpecificLogicModuleBase
{
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly JsBridge _jsBridge;
    private readonly Dispatcher _dispatcher;
    private readonly ScreenCalculator _screenCalculator;
    private readonly SocialUsersData _socialUsersData;

    //
    private WallPostContext _wallPostContext;
    private UserModel _playerModel;

    public VKLogicModule()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _jsBridge = JsBridge.Instance;
        _dispatcher = Dispatcher.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _socialUsersData = SocialUsersData.Instance;
    }

    public override async void Start()
    {
        Activate();

        await _gameStateModel.GameDataLoadedTask;

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
        _socialUsersData.NewUidsRequested += OnSocialUsersDataNewUidsRequested;
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

    private void ActivateAfterLoad()
    {
        _playerModel = _playerModelHolder.UserModel;
        _playerModel.ProgressModel.LevelChanged += OnLevelChanged;
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
                new ProcessShowAdsResultCommand().Execute(message);
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
        var rewardGold = RewardsHelper.GetGoldRewardForOfflineShare();
        
        _wallPostContext = new WallPostContext(rewardGold, buttonWorldPosition);
        var payload = new PostOfflineRevenueJsPayload(
            offlineReportPopupModel.ReportModel.CalculationHours,
            offlineReportPopupModel.ReportModel.CalculationMinutes,
            offlineReportPopupModel.ProfitFromSell);
        _jsBridge.SendCommandToJs("PostOfflineRevenue", payload);
    }
}

public struct JsCommonCommandDto
{
    public string command;
}

public struct LevelUpJsPayload
{
    public int level;

    public LevelUpJsPayload(int value)
    {
        level = value;
    }
}

public struct PostNewLevelJsPayload
{
    public int level;

    public PostNewLevelJsPayload(int value)
    {
        level = value;
    }
}

public struct NotifyInactiveFriendJsPayload
{
    public string uid;

    public NotifyInactiveFriendJsPayload(string friendUid)
    {
        uid = friendUid;
    }
}

public struct GetUsersDataJsPayload
{
    public string[] uids;

    public GetUsersDataJsPayload(string[] uids)
    {
        this.uids = uids;
    }
}

public struct PostOfflineRevenueJsPayload
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

public struct BuyVkMoneyPayload
{
    public string product;

    public BuyVkMoneyPayload(string id)
    {
        product = id;
    }
}

public struct InviteVkFriendPayload
{
    public string uid;
}

public class WallPostContext
{
    public readonly Vector3 ShareButtonWorldPosition;
    public readonly int rewardAmountGold;

    public WallPostContext(int rewardGold, Vector3 shareButtonWorldPosition)
    {
        rewardAmountGold = rewardGold;
        ShareButtonWorldPosition = shareButtonWorldPosition;
    }
}