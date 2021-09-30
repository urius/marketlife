public struct ProcessBottomPanelFriendClickCommand
{
    public void Execute(FriendData friendData)
    {
        if (friendData.IsApp)
        {
            new SwitchToFriendShopCommand().Execute(friendData);
        }
        else
        {
            new UIBottomPanelInviteFriendClickedCommand().Execute(friendData);
        }
    }
}
