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
                if (CheckCanSpendOrBlink(floorConfig.price))
                {
                    gameStateModel.SetPlacingFloor(numericId);
                }
                break;
            case ShopDecorationObjectType.Wall:
                var wallConfig = GameConfigManager.Instance.MainConfig.GetWallConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(wallConfig.price))
                {
                    gameStateModel.SetPlacingWall(numericId);
                }
                break;
            case ShopDecorationObjectType.Window:
                var windowConfig = GameConfigManager.Instance.MainConfig.GetWindowConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(windowConfig.price))
                {
                    gameStateModel.SetPlacingWindow(numericId);
                }
                break;
            case ShopDecorationObjectType.Door:
                var doorConfig = GameConfigManager.Instance.MainConfig.GetDoorConfigByNumericId(numericId);
                if (CheckCanSpendOrBlink(doorConfig.price))
                {
                    gameStateModel.SetPlacingDoor(numericId);
                }
                break;
        }
    }

    private bool CheckCanSpendOrBlink(string price)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.PlayerShopModel.CanSpendMoney(price))
        {
            return true;
        }
        else
        {
            if (Price.FromString(price).IsGold)
            {
                Dispatcher.Instance.UIRequestBlinkGold();
            }
            else
            {
                Dispatcher.Instance.UIRequestBlinkCash();
            }
            return false;
        }
    }
}
