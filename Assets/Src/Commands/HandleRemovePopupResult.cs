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
            var shopObjectModel = (gameStateModel.ShowingPopupModel as RemoveShopObjectPopupViewModel).ShopObjectModel;
            shopModel.RemoveShopObject(shopObjectModel);

            shopModel.ProgressModel.AddCash(shopObjectModel.SellPrice);
            dispatcher.UIRequestFlyingPrice(screenCalculator.CellToScreenPoint(shopObjectModel.Coords), false, shopObjectModel.SellPrice);
        }

        gameStateModel.RemoveCurrentPopupIfNeeded();
    }
}
