public class BankPopupViewModel : PopupViewModelBase
{
    public readonly int InitialTabIndex;

    public override PopupType PopupType => PopupType.Bank;

    public BankPopupViewModel(int initialTabIndex)
    {
        InitialTabIndex = initialTabIndex;
    }
}
