public struct UIBottomPanelInviteFriendClickedCommand
{
    public void Execute(FriendData friendData)
    {
        JsBridge.Instance.SendCommandToJs("InviteFriend", new InviteVkFriendPayload() { uid = friendData.Uid });
    }
}

public struct InviteVkFriendPayload
{
    public string uid;
}
