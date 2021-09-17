using Newtonsoft.Json;

public struct ProcessBuyVkMoneyResultCommand
{
    public void Execute(string message)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;
        var deserialized = JsonConvert.DeserializeObject<BuyVkMoneyResultDto>(message);

        if (deserialized.data.is_success)
        {
            if (gameStateModel.ChargedBankItem != null)
            {
                var bankItemConfig = gameStateModel.ChargedBankItem;
                if (bankItemConfig.IsGold)
                {
                    playerModelHolder.UserModel.AddGold(bankItemConfig.Value);
                }
                else
                {
                    playerModelHolder.UserModel.AddCash(bankItemConfig.Value);
                }
            }
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
        else if (deserialized.data.is_user_cancelled == false)
        {
            //failed to buy
        }

        gameStateModel.ChargedBankItem = null;
    }
}

public struct BuyVkMoneyResultDto
{
    public string command;
    public BuyVkMoneyResultPayloadDto data;
}

public struct BuyVkMoneyResultPayloadDto
{
    public bool is_success;
    public string order_id;
    public bool is_user_cancelled;
}