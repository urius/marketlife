using System;
using UnityEngine;

public class UIBottomPanelFriendShopTabMediator : UIBottomPanelTabMediatorBase
{
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly FriendsDataHolder _friendsDataHolder;
    private readonly AvatarsManager _avatarsmanager;

    //
    private UIBottomPanelFriendsTabView _tabView;
    private FriendData _viewingFriendSocialData;

    public UIBottomPanelFriendShopTabMediator(BottomPanelView view) : base(view)
    {
        _prefabsHolder = PrefabsHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _friendsDataHolder = FriendsDataHolder.Instance;
        _avatarsmanager = AvatarsManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

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
        _avatarsmanager.AvatarLoadedForId += OnAvatarLoaded;
    }

    private void Deactivate()
    {
        _avatarsmanager.AvatarLoadedForId -= OnAvatarLoaded;
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
    }

    private void SetupAvatarIcon()
    {
        _tabView.SetIconSprite(_avatarsmanager.GetAvatarSprite(_viewingFriendSocialData.Uid));
    }
}
