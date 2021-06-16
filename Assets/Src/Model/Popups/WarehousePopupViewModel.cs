public class WarehousePopupViewModel : PopupViewModelBase
{
    public readonly ProductSlotModel TargetShelfSlot;

    public WarehousePopupViewModel(ProductSlotModel targetShelfSlot)
    {
        TargetShelfSlot = targetShelfSlot;
    }

    public override PopupType PopupType => PopupType.Warehouse;
}
