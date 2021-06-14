using UnityEngine;

public struct HandleRemovePopupResult
{
    public void Execute(bool result)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.PlayerShopModel;
        var dispatcher = Dispatcher.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        if (result)
        {
            var popupModel = gameStateModel.ShowingPopupModel;
            Vector2Int coords = Vector2Int.zero;
            if (popupModel is RemoveShopObjectPopupViewModel removeShopObjectPopupViewModel)
            {
                var shopObjectModel = removeShopObjectPopupViewModel.ShopObjectModel;
                shopModel.RemoveShopObject(shopObjectModel);
                coords = shopObjectModel.Coords;
            }
            else if (popupModel is RemoveShopDecorationPopupViewModel removeShopDecorationPopupViewModel)
            {
                coords = removeShopDecorationPopupViewModel.ObjectCoords;
                var removeResult = shopModel.TryRemoveDecoration(coords);
                if (removeResult == false) return;
            }
            else if (popupModel is RemoveProductPopupViewModel removeProductPopupViewModel)
            {
                shopModel.WarehouseModel.Slots[removeProductPopupViewModel.SlotIndex].RemoveProduct();
            }

            if (popupModel is IConfirmRemoveWithRefundPopupViewModel confirmRemoveWithRefundViewModel)
            {
                shopModel.ProgressModel.AddCash(confirmRemoveWithRefundViewModel.SellPrice);
                dispatcher.UIRequestFlyingPrice(screenCalculator.CellToScreenPoint(coords), false, confirmRemoveWithRefundViewModel.SellPrice);
            }
        }
    }
}
