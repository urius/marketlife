public struct QuickDeliverCommand
{
    public void Execute(int slotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var dispatcher = Dispatcher.Instance;
        var shopModel = gameStateModel.PlayerShopModel;
        var slotModel = shopModel.WarehouseModel.Slots[slotIndex];
        var mainConfig = GameConfigManager.Instance.MainConfig;

        var deltaTimeSec = slotModel.Product.DeliverTime - gameStateModel.ServerTime;
        if (deltaTimeSec > 0)
        {
            var price = CalculationHelper.GetPriceForDeliver(mainConfig.QuickDeliverPriceGoldPerMinute, deltaTimeSec);
            if (shopModel.ProgressModel.TrySpendMoney(price))
            {
                slotModel.Product.DeliverTime = 0;
            }
            else
            {
                dispatcher.UIRequestBlinkMoney(price.IsGold);
            }
        }
    }
}
