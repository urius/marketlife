using System;

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
                if (gameStateModel.ViewingShopModel.TryPlaceFloor(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
                break;
            case PlacingStateName.PlacingNewWall:
                if (gameStateModel.ViewingShopModel.TryPlaceWall(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
                break;
            case PlacingStateName.PlacingNewWindow:
                if (gameStateModel.ViewingShopModel.TryPlaceWindow(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
                break;
            case PlacingStateName.PlacingNewDoor:
                if (gameStateModel.ViewingShopModel.TryPlaceDoor(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
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
}
