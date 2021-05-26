public struct StartMovingHighlightedObjectCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopObjectModel = gameStateModel.HighlightedShopObject;
        var shopModel = gameStateModel.PlayerShopModel;
        shopModel.RemoveShopObject(shopObjectModel);

        gameStateModel.SetPlacingObject(shopObjectModel, isNew: false);
    }
}
