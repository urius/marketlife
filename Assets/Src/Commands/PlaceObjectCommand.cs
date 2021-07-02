using System;
using UnityEngine;

public struct PlaceObjectCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var mouseCellCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
        switch (gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingNewShopObject:
                PlaceNewShopObject();
                break;
            case PlacingStateName.MovingShopObject:
                PlaceMovingShopObject();
                break;
            case PlacingStateName.PlacingNewFloor:
                PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Floor, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.PlacingNewWall:
                PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Wall, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.PlacingNewWindow:
                PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Window, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.MovingWindow:
                PlaceMovingDecoration(mouseCellCoords, ShopDecorationObjectType.Window, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.PlacingNewDoor:
                PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Door, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.MovingDoor:
                PlaceMovingDecoration(mouseCellCoords, ShopDecorationObjectType.Door, gameStateModel.PlacingDecorationNumericId);
                break;
            case PlacingStateName.PlacingProduct:
                PlaceProductOnHighlightedShelf();
                break;
        }
    }

    private void PlaceProductOnHighlightedShelf()
    {
        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var highlightState = gameStateModel.HighlightState;
        var highlightedShopObject = highlightState.HighlightedShopObject;
        var loc = LocalizationManager.Instance;
        var screenCalculator = ScreenCalculator.Instance;

        if (highlightState.IsHighlighted
            && highlightedShopObject != null
            && highlightedShopObject.Type == ShopObjectType.Shelf)
        {
            var shelf = highlightedShopObject as ShelfModel;
            var placingProduct = shopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex].Product;
            string flyingTextToShow = null;
            for (var i = 0; i < shelf.PartsCount; i++)
            {
                flyingTextToShow = null;
                if (shelf.TryGetProductAt(i, out var productOnShelf))
                {
                    if (productOnShelf.NumericId == placingProduct.NumericId)
                    {
                        var neededAmount = Math.Min(shelf.GetRestAmountOn(i), placingProduct.Amount);
                        if (neededAmount > 0)
                        {
                            productOnShelf.Amount += neededAmount;
                            placingProduct.Amount -= neededAmount;
                            break;
                        }
                        else
                        {
                            flyingTextToShow = loc.GetLocalization(LocalizationKeys.FlyingTextShelfDoesntHaveFreeSpace);
                        }
                    }
                    else
                    {
                        flyingTextToShow = loc.GetLocalization(LocalizationKeys.FlyingTextShelfDoesntHaveFreeSpace);
                    }
                }
                else
                {
                    var neededAmount = Math.Min(shelf.PartVolume / placingProduct.Config.Volume, placingProduct.Amount);
                    var productToPlace = new ProductModel(placingProduct.Config, neededAmount);
                    if (shelf.TrySetProductOn(i, productToPlace))
                    {
                        placingProduct.Amount -= neededAmount;
                        break;
                    }
                }
            }

            if (placingProduct.Amount <= 0)
            {
                gameStateModel.ResetPlacingState();
            }

            if (flyingTextToShow != null)
            {
                dispatcher.UIRequestFlyingText(screenCalculator.CellToScreenPoint(shelf.Coords), flyingTextToShow);
            }
        }
    }

    private void PlaceNewShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var shopObject = gameStateModel.PlacingShopObjectModel;
        var screenCalculator = ScreenCalculator.Instance;
        var dispatcher = Dispatcher.Instance;
        var localiationManager = LocalizationManager.Instance;

        var screenCoords = screenCalculator.CellToScreenPoint(shopObject.Coords);
        if (shopModel.CanPlaceShopObject(shopObject))
        {
            var price = shopObject.Price;
            if (TrySpendMoneyOrBlink(price))
            {
                dispatcher.UIRequestFlyingPrice(screenCoords, price.IsGold, -price.Value);

                var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
                shopModel.PlaceShopObject(clonedShopObject);
            }
            else
            {
                gameStateModel.ResetPlacingState();
                dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextInsufficientFunds));
            }
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
        }
    }

    private void PlaceMovingShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;

        var shopObject = gameStateModel.PlacingShopObjectModel;
        if (shopModel.CanPlaceShopObject(shopObject))
        {
            shopModel.PlaceShopObject(shopObject);
            gameStateModel.ResetPlacingState();
        }
    }

    private void PlaceNewDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var screenCalculator = ScreenCalculator.Instance;
        var decorationConfig = mainConfig.GetDecorationConfigBuNumericId(decorationType, numericId);
        var dispatcher = Dispatcher.Instance;
        var localiationManager = LocalizationManager.Instance;

        var screenCoords = screenCalculator.CellToScreenPoint(coords);
        if (shopModel.CanPlaceDecoration(decorationType, coords, numericId))
        {
            var price = decorationConfig.Price;
            if (TrySpendMoneyOrBlink(price))
            {
                dispatcher.UIRequestFlyingPrice(screenCoords, price.IsGold, -price.Value);

                shopModel.TryPlaceDecoration(decorationType, coords, numericId);
            }
            else
            {
                gameStateModel.ResetPlacingState();
                dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextInsufficientFunds));
            }
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
        }
    }

    private void PlaceMovingDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;

        if (shopModel.TryPlaceDecoration(decorationType, coords, numericId))
        {
            gameStateModel.ResetPlacingState();
        }
    }

    private bool TrySpendMoneyOrBlink(Price price)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shopModel = playerModel.ShopModel;
        if (playerModel.TrySpendMoney(price))
        {
            return true;
        }
        else
        {
            Dispatcher.Instance.UIRequestBlinkMoney(price.IsGold);
            return false;
        }
    }
}