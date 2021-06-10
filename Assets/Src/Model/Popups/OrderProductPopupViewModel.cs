public class OrderProductPopupViewModel : PopupViewModelBase
{
    private readonly int _targetWarehouseSlotIndex;

    public OrderProductPopupViewModel(int targetWarehouseSlotIndex)
    {
        _targetWarehouseSlotIndex = targetWarehouseSlotIndex;
    }

    public override PopupType PopupType => PopupType.OrderProduct;
}
