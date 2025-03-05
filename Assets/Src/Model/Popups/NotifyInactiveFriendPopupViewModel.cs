namespace Src.Model.Popups
{
    public class NotifyInactiveFriendPopupViewModel : ConfirmPopupViewModel
    {
        public readonly string FriendUid;

        public NotifyInactiveFriendPopupViewModel(string friendUid, string titleText, string messageText)
            : base(titleText, messageText)
        {
            FriendUid = friendUid;
        }
    }
}
