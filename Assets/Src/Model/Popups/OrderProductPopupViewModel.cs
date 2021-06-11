public class OrderProductPopupViewModel : PopupViewModelBase
{
    public readonly int TargetWarehouseSlotIndex;

    public OrderProductPopupViewModel(int targetWarehouseSlotIndex)
    {
        TargetWarehouseSlotIndex = targetWarehouseSlotIndex;
    }

    public override PopupType PopupType => PopupType.OrderProduct;
}
