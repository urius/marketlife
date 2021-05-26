public struct StartMovingHighlightedObjectCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var highlightState = gameStateModel.HighlightState;
        var shopModel = gameStateModel.PlayerShopModel;

        if (gameStateModel.PlacingState != PlacingStateName.None) return;

        if (highlightState.HighlightedShopObject != null)
        {
            var shopObjectModel = highlightState.HighlightedShopObject;
            shopModel.RemoveShopObject(shopObjectModel);

            gameStateModel.SetPlacingObject(shopObjectModel, isNew: false);
        }
        else if (highlightState.IsHighlightedDecoration)
        {
            var coords = highlightState.HighlightedDecorationCoords;
            var shopDesign = shopModel.ShopDesign;
            var decorationType = shopDesign.GetDecorationType(coords);
            int numericId;
            switch (decorationType)
            {
                case ShopDecorationObjectType.Window:
                    numericId = shopDesign.Windows[coords];
                    shopDesign.RemoveWindow(coords);
                    gameStateModel.SetPlacingWindow(numericId, isNew: false);
                    break;
                case ShopDecorationObjectType.Door:
                    numericId = shopDesign.Doors[coords];
                    shopDesign.RemoveDoor(coords);
                    gameStateModel.SetPlacingDoor(numericId, isNew: false);
                    break;
            }
        }
        gameStateModel.ResetHighlightedState();
    }
}
