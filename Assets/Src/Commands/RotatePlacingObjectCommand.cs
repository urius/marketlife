public struct RotatePlacingObjectCommand
{
    public void Execute(bool isRightRotation)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.PlacingShopObjectModel != null)
        {
            gameStateModel.PlacingShopObjectModel.Side += isRightRotation ? 1 : -1;
        }
    }
}
