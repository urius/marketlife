using System.Collections.Generic;

public class UIBottomPanelFriendsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<UIBottomPanelFriendItemView, FriendData>
{
    private readonly FriendsDataHolder _friendsDataHolder;

    public UIBottomPanelFriendsTabMediator(BottomPanelView view) : base(view)
    {
        _friendsDataHolder = FriendsDataHolder.Instance;
    }

    protected override UIBottomPanelFriendItemView GetOrCreateItem()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerable<FriendData> GetViewModelsToShow()
    {
        return _friendsDataHolder.Friends;
    }

    protected override void HandleClick(FriendData viewModel)
    {
        throw new System.NotImplementedException();
    }

    protected override void ReturnOrDestroyScrollBoxItem(UIBottomPanelFriendItemView itemView)
    {
        throw new System.NotImplementedException();
    }

    protected override void SetupItem(UIBottomPanelFriendItemView itemView, FriendData viewModel)
    {
        throw new System.NotImplementedException();
    }
}
