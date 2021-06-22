public class RemoveProductPopupViewModel : ConfirmPopupViewModel
{
    public readonly int SlotIndex;

    public RemoveProductPopupViewModel(string titleText, string messageText, int slotIndex)
        : base(titleText, messageText)
    {
        SlotIndex = slotIndex;
    }
}
