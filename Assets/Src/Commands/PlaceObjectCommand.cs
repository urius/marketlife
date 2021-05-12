public struct PlaceObjectCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        switch (gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingShopObject:
                var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
                if (gameStateModel.ViewingShopModel.CanPlaceShopObject(clonedShopObject))
                {
                    gameStateModel.ViewingShopModel.PlaceShopObject(clonedShopObject);
                }
                break;
        }
    }
}
