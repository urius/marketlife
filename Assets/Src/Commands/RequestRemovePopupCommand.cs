using Src.Common;
using Src.Managers;

public struct RequestRemovePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;
        var shopModel = playerModelHolder.ShopModel;
        var loc = LocalizationManager.Instance;

        if (gameStateModel.HighlightState.IsHighlighted)
        {
            var highlightState = gameStateModel.HighlightState;
            gameStateModel.ResetHighlightedState();
            if (highlightState.HighlightedShopObject != null)
            {
                var sellPrice = CalculationHelper.GetSellPrice(highlightState.HighlightedShopObject.Price);
                gameStateModel.ResetHighlightedState();
                var tittleText = loc.GetLocalization(LocalizationKeys.PopupRemoveObjectTitle);
                var messageText = string.Format(loc.GetLocalization(LocalizationKeys.PopupRemoveObjectText), sellPrice);
                gameStateModel.ShowPopup(new RemoveShopObjectPopupViewModel(tittleText, messageText, highlightState.HighlightedShopObject, sellPrice));
            }
            else
            {
                var shopDesign = shopModel.ShopDesign;
                var coords = highlightState.HighlightedCoords;
                var decorationType = shopDesign.GetDecorationType(coords);
                if (decorationType == ShopDecorationObjectType.Door
                    && shopDesign.Doors.Count <= 1)
                {
                    var dispatcher = Dispatcher.Instance;
                    var screenCalculator = ScreenCalculator.Instance;
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
                    var sellPrice = CalculationHelper.GetSellPrice(originalPrice);
                    var tittleText = loc.GetLocalization(LocalizationKeys.PopupRemoveObjectTitle);
                    var messageText = string.Format(loc.GetLocalization(LocalizationKeys.PopupRemoveObjectText), sellPrice);
                    gameStateModel.ShowPopup(new RemoveShopDecorationPopupViewModel(tittleText, messageText, coords, sellPrice));
                }
            }
        }
    }

    public void Execute(int slotIndex)
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var gameStateModel = GameStateModel.Instance;
        var slotModel = playerModelHolder.ShopModel.WarehouseModel.Slots[slotIndex];
        var loc = LocalizationManager.Instance;

        var titletext = loc.GetLocalization(LocalizationKeys.PopupRemoveProductTitle);
        var productName = loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{slotModel.Product.NumericId}");
        var messageText = string.Format(loc.GetLocalization(LocalizationKeys.PopupRemoveProductText), productName);

        gameStateModel.ShowPopup(new RemoveProductPopupViewModel(titletext, messageText, slotIndex));
    }
}
