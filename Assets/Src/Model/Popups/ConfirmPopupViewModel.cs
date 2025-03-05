namespace Src.Model.Popups
{
    public class ConfirmPopupViewModel : PopupViewModelBase
    {
        public readonly string TitleText;
        public readonly string MessageText;

        public ConfirmPopupViewModel(string titleText, string messageText)
        {
            TitleText = titleText;
            MessageText = messageText;
        }

        public override PopupType PopupType => PopupType.Confirm;
    }
}
