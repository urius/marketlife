public struct PlaceObjectCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var mouseCellCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
        switch (gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingShopObject:
                var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
                if (gameStateModel.ViewingShopModel.CanPlaceShopObject(clonedShopObject))
                {
                    gameStateModel.ViewingShopModel.PlaceShopObject(clonedShopObject);
                }
                break;
            case PlacingStateName.PlacingFloor:
                if (gameStateModel.ViewingShopModel.TryPlaceFloor(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
                break;
            case PlacingStateName.PlacingWall:
                if (gameStateModel.ViewingShopModel.TryPlaceWall(mouseCellCoords, gameStateModel.PlacingDecorationNumericId))
                {
                    //TODO animate money spend
                }
                break;
        }
    }
}
