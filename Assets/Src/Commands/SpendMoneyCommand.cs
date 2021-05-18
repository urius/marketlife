public struct SpendMoneyCommand
{
    public bool Execute(string priceStr)
    {
        var gameStateModel = GameStateModel.Instance;
        if (gameStateModel.PlayerShopModel.TrySpendMoney(priceStr))
        {
            return true;
        }
        else
        {
            Dispatcher.Instance.NotifyNotEnoughtMoney();
            return false;
        }
    }
}
