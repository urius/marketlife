public class ErrorPopupViewModel : PopupViewModelBase
{
    public readonly string ErrorText;

    public ErrorPopupViewModel(string text)
    {
        ErrorText = text;
    }

    public override PopupType PopupType => PopupType.Error;
}
