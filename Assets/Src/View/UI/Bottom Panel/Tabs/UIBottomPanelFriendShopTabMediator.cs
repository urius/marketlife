using UnityEngine;

public class UIBottomPanelFriendShopTabMediator : UIBottomPanelTabMediatorBase
{
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly FriendsDataHolder _friendsDataHolder;
    private readonly AvatarsManager _avatarsManager;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly MainConfig _mainConfig;
    private readonly Dispatcher _dispatcher;
    private readonly UpdatesProvider _updatesProvider;

    //
    private UIBottomPanelFriendsTabView _tabView;
    private AvailableFriendShopActionsDataModel _playerAvailableActionsDataModel;
    private FriendData _viewingFriendSocialData;

    public UIBottomPanelFriendShopTabMediator(BottomPanelView view) : base(view)
    {
        _prefabsHolder = PrefabsHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _friendsDataHolder = FriendsDataHolder.Instance;
        _avatarsManager = AvatarsManager.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _mainConfig = GameConfigManager.Instance.MainConfig;
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        _playerAvailableActionsDataModel = _playerModelHolder.UserModel.ActionsDataModel;
        _viewingFriendSocialData = _friendsDataHolder.GetFriendData(_gameStateModel.ViewingUserModel.Uid);
        var tabViewGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelFriendTabPrefab, View.transform);
        _tabView = tabViewGo.GetComponent<UIBottomPanelFriendsTabView>();
        SetupView();

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        if (_tabView != null)
        {
            GameObject.Destroy(_tabView.gameObject);
            _tabView = null;
        }

        base.Unmediate();
    }

    private void Activate()
    {
        _avatarsManager.AvatarLoadedForId += OnAvatarLoaded;
        _tabView.TakeActionView.Clicked += OnTakeActionClicked;
        _tabView.TakeActionView.BuyButtonClicked += OnBuyTakeActionClicked;
        _tabView.UnwashActionView.Clicked += OnAddUnwashActionClicked;
        _tabView.UnwashActionView.BuyButtonClicked += OnBuyAddUnwashActionClicked;
        _playerAvailableActionsDataModel.ActionDataAmountChanged += OnActionDataAmountChanged;
        _playerAvailableActionsDataModel.ActionDataCooldownTimestampChanged += OnActionDataCooldownTimestampChanged;
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
    }

    private void Deactivate()
    {
        _avatarsManager.AvatarLoadedForId -= OnAvatarLoaded;
        _tabView.TakeActionView.Clicked -= OnTakeActionClicked;
        _tabView.TakeActionView.BuyButtonClicked -= OnBuyTakeActionClicked;
        _tabView.UnwashActionView.Clicked -= OnAddUnwashActionClicked;
        _tabView.UnwashActionView.BuyButtonClicked -= OnBuyAddUnwashActionClicked;
        _playerAvailableActionsDataModel.ActionDataAmountChanged -= OnActionDataAmountChanged;
        _playerAvailableActionsDataModel.ActionDataCooldownTimestampChanged -= OnActionDataCooldownTimestampChanged;
        _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
    }

    private void OnRealtimeSecondUpdate()
    {
        SetupActionButtonView(FriendShopActionId.Take);
        SetupActionButtonView(FriendShopActionId.AddUnwash);
    }

    private void OnActionDataAmountChanged(AvailableFriendShopActionData actionData)
    {
        var view = GetViewByActionId(actionData.ActionId);
        if (view != null)
        {
            UpdateAmount(view, actionData);
        }
    }

    private void OnActionDataCooldownTimestampChanged(AvailableFriendShopActionData actionData)
    {
        SetupActionButtonView(actionData.ActionId);
    }

    private UIBottomPanelFriendTabActionButtonView GetViewByActionId(FriendShopActionId actionId)
    {
        return actionId switch
        {
            FriendShopActionId.Take => _tabView.TakeActionView,
            FriendShopActionId.AddUnwash => _tabView.UnwashActionView,
            _ => null,
        };
    }

    private void OnTakeActionClicked()
    {
        _dispatcher.UIBottomPanelFriendShopActionClicked(FriendShopActionId.Take);
    }

    private void OnBuyTakeActionClicked()
    {
        _dispatcher.UIBottomPanelBuyFriendShopActionClicked(FriendShopActionId.Take);
    }

    private void OnAddUnwashActionClicked()
    {
        _dispatcher.UIBottomPanelFriendShopActionClicked(FriendShopActionId.AddUnwash);
    }

    private void OnBuyAddUnwashActionClicked()
    {
        _dispatcher.UIBottomPanelBuyFriendShopActionClicked(FriendShopActionId.AddUnwash);
    }

    private void OnAvatarLoaded(string uid)
    {
        if (uid == _viewingFriendSocialData.Uid)
        {
            SetupAvatarIcon();
        }
    }

    private void SetupView()
    {
        _tabView.SetNameText(_viewingFriendSocialData.FirstName);
        SetupAvatarIcon();
        var friendShopUserModel = _viewingFriendSocialData.UserModel;
        _tabView.SetLevelText(friendShopUserModel.ProgressModel.Level.ToString());
        _tabView.SetExpText(FormattingHelper.ToCommaSeparatedNumber(friendShopUserModel.ProgressModel.ExpAmount));
        _tabView.SetCashText(FormattingHelper.ToCommaSeparatedNumber(friendShopUserModel.ProgressModel.Cash));

        SetupActionButtonView(FriendShopActionId.Take);
        SetupActionButtonView(FriendShopActionId.AddUnwash);
    }

    private void SetupActionButtonView(FriendShopActionId actionId)
    {
        var actionData = _playerAvailableActionsDataModel.ActionsById[actionId];
        var actionView = GetViewByActionId(actionId);
        if (actionView != null)
        {
            if (actionData.EndCooldownTimestamp > _gameStateModel.ServerTime)
            {
                actionView.SetChargingState(true);
                UpdateCooldownTime(actionView, actionData);
                actionView.SetBuyPriceText(_mainConfig.ActionResetCooldownPrice.ToString());
            }
            else
            {
                actionView.SetChargingState(false);
                UpdateAmount(actionView, actionData);
            }
        }
    }

    private void UpdateAmount(UIBottomPanelFriendTabActionButtonView actionView, AvailableFriendShopActionData actionData)
    {
        actionView.SetAmountText(actionData.RestAmount.ToString());
    }

    private void UpdateCooldownTime(UIBottomPanelFriendTabActionButtonView actionView, AvailableFriendShopActionData actionData)
    {
        actionView.SetTimeText(FormattingHelper.ToSeparatedTimeFormat(actionData.EndCooldownTimestamp - _gameStateModel.ServerTime));
    }

    private void SetupAvatarIcon()
    {
        _tabView.SetIconSprite(_avatarsManager.GetAvatarSprite(_viewingFriendSocialData.Uid));
    }
}
