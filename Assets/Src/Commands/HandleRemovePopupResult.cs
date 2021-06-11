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
            var popupModel = gameStateModel.ShowingPopupModel as IConfirmRemoveObjectPopupViewModel;
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

            shopModel.ProgressModel.AddCash(popupModel.SellPrice);
            dispatcher.UIRequestFlyingPrice(screenCalculator.CellToScreenPoint(coords), false, popupModel.SellPrice);
        }
    }
}
