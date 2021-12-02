using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBottomPanelFriendsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<UIBottomPanelFriendItemView, BottomPanelFriendTabItemViewModel>
{
    private readonly FriendsDataHolder _friendsDataHolder;
    private readonly PoolCanvasProvider _poolCanvasProvider;
    private readonly LocalizationManager _loc;
    private readonly AvatarsManager _avatarsManager;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly ColorsHolder _colorsHolder;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly MainConfig _friendActionsConfig;
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
        _spritesProvider = SpritesProvider.Instance;
        _colorsHolder = ColorsHolder.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _friendActionsConfig = GameConfigManager.Instance.MainConfig;
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
        _friendsDataHolder.FriendsDataWasSetup += OnFriendsDataSet;
        _avatarsManager.AvatarLoadedForId += OnAvatarLoadedForId;
    }

    private void Deactivate()
    {
        _friendsDataHolder.FriendsDataWasSetup -= OnFriendsDataSet;
        _avatarsManager.AvatarLoadedForId -= OnAvatarLoadedForId;
    }

    private void OnAvatarLoadedForId(string uid)
    {
        foreach (var displayedItem in DisplayedItems)
        {
            if (displayedItem.ViewModel.HasFriendData && displayedItem.ViewModel.FriendData.Uid == uid)
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

    protected override IEnumerable<BottomPanelFriendTabItemViewModel> GetViewModelsToShow()
    {
        return _friendsDataHolder.Friends
            .Where(m => m.IsApp)
            .Select(m => new BottomPanelFriendTabItemViewModel(m))
            .Concat(new BottomPanelFriendTabItemViewModel[] { new BottomPanelFriendTabItemViewModel() });
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

    protected override void SetupItem(UIBottomPanelFriendItemView itemView, BottomPanelFriendTabItemViewModel viewModel)
    {
        if (viewModel.HasFriendData)
        {
            var friendData = viewModel.FriendData;
            itemView.SetTextDefaultColor();
            itemView.SetTopText(friendData.FirstName);
            itemView.SetBgImageDefaultColor();
            itemView.SetBottomButtonEnabled(false);

            //if (friendData.IsApp)
            {
                if (friendData.IsInactive())
                {
                    itemView.SetBgImageColor(_colorsHolder.BottomPanelFriendsInactiveFriendButtonColor);
                    itemView.SetStatusIconImageSprite(_spritesProvider.GetMoonIcon());
                    itemView.SetMainHintEnabled(true);
                    itemView.SetMainHintText(_loc.GetLocalization(LocalizationKeys.HintBottomPanelInactiveFriend));
                }
                else
                {
                    var friendActionsModel = _playerModelHolder.UserModel.FriendsActionsDataModels.GetFriendShopActionsModel(friendData.Uid);
                    var isVisitBonusAvailable = _friendActionsConfig.IsEnabled(FriendShopActionId.VisitBonus)
                        && friendActionsModel.ActionsById[FriendShopActionId.VisitBonus].IsAvailable(_gameStateModel.ServerTime);
                    itemView.SetStatusIconImageSprite(isVisitBonusAvailable ? _spritesProvider.GetGoldIcon() : null);
                    itemView.SetMainHintEnabled(true);
                    itemView.SetMainHintText(_loc.GetLocalization(LocalizationKeys.HintBottomPanelVisitFriend));
                }
            }

            var avatarSprite = _avatarsManager.GetAvatarSprite(friendData.Uid);
            itemView.SetMainIconImageSprite(avatarSprite);
            if (avatarSprite == null)
            {
                _avatarsManager.LoadAvatarForUid(friendData.Uid);
            }
        }
        else
        {
            itemView.SetTextAltColor();
            itemView.SetBgImageColor(_colorsHolder.BottomPanelFriendsTabAddButtonColor);
            itemView.SetTopText(_loc.GetLocalization(LocalizationKeys.CommonInvite));
            itemView.SetBottomButtonEnabled(false);
            itemView.SetMainHintEnabled(true);
            itemView.SetMainHintText(_loc.GetLocalization(LocalizationKeys.HintBottomPanelInviteFriends));
            itemView.SetMainIconImageSprite(_spritesProvider.GetBigPlusSignIcon());
            itemView.SetStatusIconImageSprite(null);
        }
    }

    protected override void HandleClick(BottomPanelFriendTabItemViewModel viewModel)
    {
        if (viewModel.HasFriendData)
        {
            _dispatcher.UIBottomPanelFriendClicked(viewModel.FriendData);
        }
        else
        {
            _dispatcher.UIBottomPanelInviteFriendsClicked();
        }
    }

    private void OnFriendsDataSet()
    {
        RefreshScrollContent();
    }
}

public class BottomPanelFriendTabItemViewModel
{
    public readonly FriendData FriendData;

    public BottomPanelFriendTabItemViewModel(FriendData friendData = null)
    {
        FriendData = friendData;
    }

    public bool HasFriendData => FriendData != null;
}