using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelFriendsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<UIBottomPanelFriendItemView, FriendData>
{
    private readonly FriendsDataHolder _friendsDataHolder;
    private readonly PoolCanvasProvider _poolCanvasProvider;
    private readonly LocalizationManager _loc;
    private readonly AvatarsManager _avatarsManager;
    private readonly Dispatcher _dispatcher;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly Queue<UIBottomPanelFriendItemView> _cachedViews = new Queue<UIBottomPanelFriendItemView>();

    public UIBottomPanelFriendsTabMediator(BottomPanelView view) : base(view)
    {
        _prefabsHolder = PrefabsHolder.Instance;
        _friendsDataHolder = FriendsDataHolder.Instance;
        _poolCanvasProvider = PoolCanvasProvider.Instance;
        _loc = LocalizationManager.Instance;
        _avatarsManager = AvatarsManager.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        View.SetButtonSelected(View.FriendsButton);
        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();
        View.SetButtonUnselected(View.FriendsButton);

        base.Unmediate();

        DestroyCashedViews();
    }

    private void Activate()
    {
        _friendsDataHolder.FriendsDataIsSet += OnFriendsDataSet;
        _avatarsManager.AvatarLoadedForId += OnAvatarLoadedForId;
    }

    private void Deactivate()
    {
        _friendsDataHolder.FriendsDataIsSet -= OnFriendsDataSet;
        _avatarsManager.AvatarLoadedForId -= OnAvatarLoadedForId;
    }

    private void OnAvatarLoadedForId(string uid)
    {
        foreach (var displayedItem in DisplayedItems)
        {
            if (displayedItem.ViewModel.Uid == uid)
            {
                displayedItem.View.SetMainIconImageSprite(_avatarsManager.GetAvatarSprite(uid));
                break;
            }
        }
    }

    private void DestroyCashedViews()
    {
        while (_cachedViews.Count > 0)
        {
            var viewItem = _cachedViews.Dequeue();
            GameObject.Destroy(viewItem.gameObject);
        }
    }

    protected override IEnumerable<FriendData> GetViewModelsToShow()
    {
        return _friendsDataHolder.Friends;
    }

    protected override UIBottomPanelFriendItemView GetOrCreateItem()
    {
        UIBottomPanelFriendItemView result;
        if (_cachedViews.Count > 0)
        {
            result = _cachedViews.Dequeue();
        }
        else
        {
            var itemViewGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelFriendScrollItemPrefab, _poolCanvasProvider.PoolCanvasTransform);
            result = itemViewGo.GetComponent<UIBottomPanelFriendItemView>();
        }
        result.gameObject.SetActive(true);
        result.transform.SetParent(View.ScrollBoxView.Content);
        return result;
    }

    protected override void ReturnOrDestroyScrollBoxItem(UIBottomPanelFriendItemView itemView)
    {
        itemView.gameObject.SetActive(false);
        itemView.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
        _cachedViews.Enqueue(itemView);
    }

    protected override void SetupItem(UIBottomPanelFriendItemView itemView, FriendData viewModel)
    {
        itemView.SetTopText(viewModel.FirstName);
        if (viewModel.IsApp)
        {
            itemView.SetBottomButtonEnabled(false);
            itemView.SetMainHintEnabled(true);
            itemView.SetMainHintText(_loc.GetLocalization(LocalizationKeys.HintBottomPanelVisitFriend));
        }
        else
        {
            itemView.SetBottomButtonEnabled(true);
            itemView.SetMainHintEnabled(false);
        }

        var avatarSprite = _avatarsManager.GetAvatarSprite(viewModel.Uid);
        itemView.SetMainIconImageSprite(avatarSprite);
        if (avatarSprite == null)
        {
            _avatarsManager.LoadAvatarForUid(viewModel.Uid);
        }
    }

    protected override void ActivateItem(UIBottomPanelFriendItemView itemView, FriendData viewModel)
    {
        base.ActivateItem(itemView, viewModel);

        itemView.BottomButtonClicked += OnBottomButtonClicked;
    }

    protected override void DeactivateItem(UIBottomPanelFriendItemView itemView, FriendData viewModel)
    {
        itemView.BottomButtonClicked -= OnBottomButtonClicked;

        base.DeactivateItem(itemView, viewModel);
    }

    protected override void HandleClick(FriendData viewModel)
    {
        _dispatcher.UIBottomPanelFriendClicked(viewModel);
    }

    private void OnBottomButtonClicked(UIBottomPanelFriendItemView view)
    {
        foreach (var displayedItem in DisplayedItems)
        {
            if (displayedItem.View == view)
            {
                _dispatcher.UIBottomPanelInviteFriendClicked(displayedItem.ViewModel);
                break;
            }
        }
    }

    private void OnFriendsDataSet()
    {
        RefreshScrollContent();
    }
}
