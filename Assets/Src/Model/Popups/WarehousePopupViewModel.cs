using Src.Model.ShopObjects;

namespace Src.Model.Popups
{
    public class WarehousePopupViewModel : PopupViewModelBase
    {
        public override PopupType PopupType => PopupType.Warehouse;
    }

    public class WarehousePopupForShelfViewModel : PopupViewModelBase
    {
        public readonly ShelfModel TargetShelfModel;
        public readonly int TargetShelfSlotIndex;

        public WarehousePopupForShelfViewModel(ShelfModel targetShelfModel, int targetShelfSlotIndex)
        {
            TargetShelfModel = targetShelfModel;
            TargetShelfSlotIndex = targetShelfSlotIndex;
        }

        public override PopupType PopupType => PopupType.WarehouseForShelf;
    }
}