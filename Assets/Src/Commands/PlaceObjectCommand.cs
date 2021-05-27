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
        }
    }

    private void PlaceNewShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var shopObject = gameStateModel.PlacingShopObjectModel;

        if (shopModel.CanPlaceShopObject(shopObject))
        {
            if (TrySpendMoneyOrBlink(shopObject.Price))
            {
                var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
                shopModel.PlaceShopObject(clonedShopObject);
            }
            else
            {
                gameStateModel.ResetPlacingState();
            }
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
        var decorationConfig = mainConfig.GetDecorationConfigBuNumericId(decorationType, numericId);

        if (shopModel.CanPlaceDecoration(decorationType, coords, numericId))
        {
            if (TrySpendMoneyOrBlink(Price.FromString(decorationConfig.price)))
            {
                shopModel.TryPlaceDecoration(decorationType, coords, numericId);
            }
            else
            {
                gameStateModel.ResetPlacingState();
            }
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
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        if (shopModel.TrySpendMoney(price))
        {
            return true;
        }
        else
        {
            if (price.IsGold)
            {
                Dispatcher.Instance.UIRequestBlinkGold();
            }
            else
            {
                Dispatcher.Instance.UIRequestBlinkCash();
            }
            return false;
        }
    }
}