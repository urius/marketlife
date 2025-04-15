using Src.Managers;

public struct HandleBottomPanelFriendClickedCommand
{
    public void Execute(FriendData friendData)
    {
        if (friendData.IsInactive())
        {
            var gameStateModel = GameStateModel.Instance;
            var loc = LocalizationManager.Instance;
            var popupViewModel = new NotifyInactiveFriendPopupViewModel(
                friendData.Uid,
                loc.GetLocalization(LocalizationKeys.PopupNotifyInactiveFriendTitle),
                loc.GetLocalization(LocalizationKeys.PopupNotifyInactiveFriendMessage));
            gameStateModel.ShowPopup(popupViewModel);
        }
        else
        {
            new SwitchToFriendShopCommand().Execute(friendData);
        }
    }
}
