public struct RotatePlacingObjectCommand
{
    public void Execute(bool isRightRotation)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.PlacingState == PlacingStateName.PlacingShopObject)
        {
            gameStateModel.PlacingShopObjectModel.Side += isRightRotation ? 1 : -1;
        }
    }
}
