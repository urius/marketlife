public struct UIRequestPlacingDecorationCommand
{
    public void Execute(ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.PlacingState != PlacingStateName.None) return;

        Dispatcher.Instance.RequestForceMouseCellPositionUpdate();
        switch (decorationType)
        {
            case ShopDecorationObjectType.Floor:
                var floorConfig = GameConfigManager.Instance.MainConfig.GetFloorConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(floorConfig.Price))
                {
                    gameStateModel.SetPlacingFloor(numericId);
                }
                break;
            case ShopDecorationObjectType.Wall:
                var wallConfig = GameConfigManager.Instance.MainConfig.GetWallConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(wallConfig.Price))
                {
                    gameStateModel.SetPlacingWall(numericId);
                }
                break;
            case ShopDecorationObjectType.Window:
                var windowConfig = GameConfigManager.Instance.MainConfig.GetWindowConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(windowConfig.Price))
                {
                    gameStateModel.SetPlacingWindow(numericId);
                }
                break;
            case ShopDecorationObjectType.Door:
                var doorConfig = GameConfigManager.Instance.MainConfig.GetDoorConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(doorConfig.Price))
                {
                    gameStateModel.SetPlacingDoor(numericId);
                }
                break;
        }
    }

    private bool CheckCanSpendOrBlink(Price price)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        if (playerModel.CanSpendMoney(price))
        {
            return true;
        }
        else
        {
            Dispatcher.Instance.UIRequestBlinkMoney(price.IsGold);
            return false;
        }
    }
}
