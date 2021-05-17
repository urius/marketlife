public struct UIRequestPlacingDecorationCommand
{
    public void Execute(ShopDecorationObjectType decorationType, int numericId)
    {
        switch (decorationType)
        {
            case ShopDecorationObjectType.Floor:
                var floorConfig = GameConfigManager.Instance.MainConfig.GetFloorConfigByNumericId(numericId);
                var gameStateModel = GameStateModel.Instance;
                if (floorConfig.price != null)// TODO check price properly
                {
                    gameStateModel.SetPlacingFloor(numericId);
                }
                break;
        }
    }
}
