public class RemoveProductPopupViewModel : PopupViewModelBase
{
    public readonly int SlotIndex;
    public readonly int ProductNumericId;

    public RemoveProductPopupViewModel(int slotIndex, int productNumericId)
    {
        SlotIndex = slotIndex;
        ProductNumericId = productNumericId;
    }

    public override PopupType PopupType => PopupType.ConfirmRemoveObject;
}
