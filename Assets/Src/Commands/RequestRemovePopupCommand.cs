public struct RequestRemovePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.PlayerShopModel;
        if (gameStateModel.HighlightState.IsHighlighted)
        {
            var highlightState = gameStateModel.HighlightState;
            gameStateModel.ResetHighlightedState();
            if (highlightState.HighlightedShopObject != null)
            {
                var sellPrice = shopModel.GetSellPrice(highlightState.HighlightedShopObject.Price);
                gameStateModel.ShowPopup(new RemoveShopObjectPopupViewModel(highlightState.HighlightedShopObject, sellPrice));
            }
            else
            {
                var shopDesign = shopModel.ShopDesign;
                var coords = highlightState.HighlightedDecorationCoords;
                var decorationType = shopDesign.GetDecorationType(coords);
                if (decorationType == ShopDecorationObjectType.Door
                    && shopDesign.Doors.Count <= 1)
                {
                    var dispatcher = Dispatcher.Instance;
                    var screenCalculator = ScreenCalculator.Instance;
                    var loc = LocalizationManager.Instance;
                    dispatcher.UIRequestFlyingText(screenCalculator.CellToScreenPoint(coords), loc.GetLocalization(LocalizationKeys.FlyingTextCantSellLastDoor));
                }
                else
                {
                    var config = GameConfigManager.Instance.MainConfig;
                    int numericId = -1;
                    switch (decorationType)
                    {
                        case ShopDecorationObjectType.Door:
                            numericId = shopDesign.Doors[coords];
                            break;
                        case ShopDecorationObjectType.Window:
                            numericId = shopDesign.Windows[coords];
                            break;
                    }
                    var originalPrice = config.GetDecorationConfigBuNumericId(decorationType, numericId).Price;
                    gameStateModel.ShowPopup(new RemoveShopDecorationPopupViewModel(coords, shopModel.GetSellPrice(originalPrice)));
                }
            }
        }
    }
}
