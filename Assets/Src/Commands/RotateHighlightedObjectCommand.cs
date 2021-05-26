public struct RotateHighlightedObjectCommand
{
    public void Execute(int deltaSide)
    {
        var gameStateMOdel = GameStateModel.Instance;
        var shopModel = gameStateMOdel.ViewingShopModel;
        var higlightState = gameStateMOdel.HighlightState;

        if (higlightState.HighlightedShopObject != null)
        {
            var shopObjectModel = higlightState.HighlightedShopObject;
            if (shopModel.CanRotateShopObject(shopObjectModel, deltaSide))
            {
                shopModel.RotateShopObject(shopObjectModel, deltaSide);
            }
            else if (shopModel.CanRotateShopObject(shopObjectModel, 2 * deltaSide))
            {
                shopModel.RotateShopObject(shopObjectModel, 2 * deltaSide);
            }
            else
            {
                //TODO: "Can't rotate" Sound
            }
        }
    }
}
