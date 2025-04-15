using Src.Common;

public struct SpendMoneyCommand
{
    public bool Execute(string priceStr)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        if (playerModel.TrySpendMoney(priceStr))
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
