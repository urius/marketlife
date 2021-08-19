using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelFriendsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<FriendData>
{
    private readonly FriendsDataHolder _friendsDataHolder;

    public UIBottomPanelFriendsTabMediator(BottomPanelView view) : base(view)
    {
        _friendsDataHolder = FriendsDataHolder.Instance;
    }

    protected override IEnumerable<FriendData> GetViewModelsToShow()
    {
        return _friendsDataHolder.Friends;
    }

    protected override void HandleClick(FriendData viewModel)
    {
        throw new System.NotImplementedException();
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, FriendData viewModel)
    {
        throw new System.NotImplementedException();
    }
}
