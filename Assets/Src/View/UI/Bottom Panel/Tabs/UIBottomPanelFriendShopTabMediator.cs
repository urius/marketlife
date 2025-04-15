using System;
using Src.Common;
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
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly ScreenCalculator _screenCalculator;

    //
    private UIBottomPanelFriendsTabView _tabView;
    private string _viewingFriendUid;
    private FriendShopActionsModel _playerAvailableActionsDataModel;
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
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        _viewingFriendUid = _gameStateModel.ViewingUserModel.Uid;
        _playerAvailableActionsDataModel = _playerModelHolder.UserModel.FriendsActionsDataModels.GetFriendShopActionsModel(_viewingFriendUid);
        _viewingFriendSocialData = _friendsDataHolder.GetFriendData(_viewingFriendUid);
        var tabViewGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelFriendTabPrefab, View.transform);
        _tabView = tabViewGo.GetComponent<UIBottomPanelFriendsTabView>();
        SetupView();

        _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelFriendShopAddProductButton, _tabView.AddProductActionView.transform);
        _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelFriendShopTakeButton, _tabView.TakeActionView.transform);
        _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelFriendShopAddUnwashButton, _tabView.UnwashActionView.transform);

        Activate();

        _dispatcher.UIFriendShopBottomPanelUserViewCreated(_screenCalculator.WorldToScreenPoint(_tabView.IconImageTransform.position));
    }

    public override void Unmediate()
    {
        _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelFriendShopAddProductButton);
        _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelFriendShopTakeButton);
        _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelFriendShopAddUnwashButton);

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
        _tabView.AddProductActionView.Clicked += OnAddProductActionView;
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
        _tabView.AddProductActionView.Clicked -= OnAddProductActionView;
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
        SetupActionButtonView(FriendShopActionId.TakeProduct);
        SetupActionButtonView(FriendShopActionId.AddUnwash);
    }

    private void OnActionDataAmountChanged(string friendId, FriendShopActionData actionData)
    {
        var view = GetViewByActionId(actionData.ActionId);
        if (view != null)
        {
            UpdateAmount(view, actionData.RestAmount);
        }
    }

    private void OnActionDataCooldownTimestampChanged(string friendId, FriendShopActionData actionData)
    {
        SetupActionButtonView(actionData.ActionId);
    }

    private UIBottomPanelFriendTabActionButtonView GetViewByActionId(FriendShopActionId actionId)
    {
        return actionId switch
        {
            FriendShopActionId.AddProduct => _tabView.AddProductActionView,
            FriendShopActionId.TakeProduct => _tabView.TakeActionView,
            FriendShopActionId.AddUnwash => _tabView.UnwashActionView,
            _ => null,
        };
    }

    private void OnAddProductActionView()
    {
        _dispatcher.UIBottomPanelFriendShopActionClicked(FriendShopActionId.AddProduct);
    }

    private void OnTakeActionClicked()
    {
        _dispatcher.UIBottomPanelFriendShopActionClicked(FriendShopActionId.TakeProduct);
    }

    private void OnBuyTakeActionClicked()
    {
        _dispatcher.UIBottomPanelBuyFriendShopActionClicked(FriendShopActionId.TakeProduct);
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

        SetupActionButtonView(FriendShopActionId.AddProduct);
        SetupActionButtonView(FriendShopActionId.TakeProduct);
        SetupActionButtonView(FriendShopActionId.AddUnwash);
    }

    private void SetupActionButtonView(FriendShopActionId actionId)
    {
        var actionView = GetViewByActionId(actionId);
        if (_playerAvailableActionsDataModel.ActionsById.ContainsKey(actionId))
        {
            var actionData = _playerAvailableActionsDataModel.ActionsById[actionId];
            if (actionView != null)
            {
                if (actionData.EndCooldownTimestamp > _gameStateModel.ServerTime)
                {
                    actionView.SetChargingState(true);
                    UpdateCooldownTime(actionView, actionData);
                    actionView.SetBuyPriceText(_mainConfig.GetActionResetCooldownPriceGold(actionId).ToString());
                }
                else
                {
                    actionView.SetChargingState(false);
                    UpdateAmount(actionView, actionData.RestAmount);
                }
            }
        }
        else
        {
            actionView.SetChargingState(false);
            UpdateAmount(actionView, 0);
        }
    }

    private void UpdateAmount(UIBottomPanelFriendTabActionButtonView actionView, int amount)
    {
        actionView.SetAmountTextVisibility(amount > 0);
        if (amount > 0)
        {
            actionView.SetAmountText(amount.ToString());
        }
    }

    private void UpdateCooldownTime(UIBottomPanelFriendTabActionButtonView actionView, FriendShopActionData actionData)
    {
        actionView.SetTimeText(FormattingHelper.ToSeparatedTimeFormat(actionData.EndCooldownTimestamp - _gameStateModel.ServerTime));
    }

    private void SetupAvatarIcon()
    {
        _tabView.SetIconSprite(_avatarsManager.GetAvatarSprite(_viewingFriendSocialData.Uid));
    }
}
