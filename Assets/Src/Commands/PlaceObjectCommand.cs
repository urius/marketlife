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
        //todo: check enought money
        if (gameStateModel.ViewingShopModel.CanPlaceShopObject(gameStateModel.PlacingShopObjectModel))
        {
            var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
            gameStateModel.ViewingShopModel.PlaceShopObject(clonedShopObject);
        }
    }

    private void PlaceMovingShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        //todo: check enought money

        var shopObject = gameStateModel.PlacingShopObjectModel;
        if (gameStateModel.ViewingShopModel.CanPlaceShopObject(shopObject))
        {
            gameStateModel.ViewingShopModel.PlaceShopObject(shopObject);
            gameStateModel.ResetPlacingState();
        }
    }

    private void PlaceNewDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.ViewingShopModel.TryPlaceDecoration(decorationType, coords, numericId))
        {
            //TODO animate money spend
        }
    }

    private void PlaceMovingDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.ViewingShopModel.TryPlaceDecoration(decorationType, coords, numericId))
        {
            gameStateModel.ResetPlacingState();
        }
    }
}