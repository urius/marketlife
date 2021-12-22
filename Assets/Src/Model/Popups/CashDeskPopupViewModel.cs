public class CashDeskPopupViewModel : PopupViewModelBase
{
    public readonly CashDeskModel CashDeskModel;

    public CashDeskPopupViewModel(CashDeskModel cashDeskModel)
    {
        CashDeskModel = cashDeskModel;
    }

    public override PopupType PopupType => PopupType.CashDesk;
}
