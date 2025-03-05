using Src.Model;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct UIShelfContentAddProductClicked
    {
        public void Execute(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
        {
            var gameStateModel = GameStateModel.Instance;
            gameStateModel.ShowPopup(new WarehousePopupForShelfViewModel(popupModel.ShelfModel, shelfSlotIndex));
        }
    }
}
