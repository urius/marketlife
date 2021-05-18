public struct UIRequestPlacingDecorationCommand
{
    public void Execute(ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        switch (decorationType)
        {
            case ShopDecorationObjectType.Floor:
                var floorConfig = GameConfigManager.Instance.MainConfig.GetFloorConfigByNumericId(numericId);
                if (gameStateModel.PlayerShopModel.CanSpendMoney(floorConfig.price))
                {
                    gameStateModel.SetPlacingFloor(numericId);
                }
                break;
            case ShopDecorationObjectType.Wall:
                var wallConfig = GameConfigManager.Instance.MainConfig.GetWallConfigByNumericId(numericId);
                if (gameStateModel.PlayerShopModel.CanSpendMoney(wallConfig.price))
                {
                    gameStateModel.SetPlacingWall(numericId);
                }
                break;
        }
    }
}
