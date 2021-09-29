public struct UIBankItemClickedCommand
{
    public void Execute(BankConfigItem itemConfig)
    {
        var gameStateModel = GameStateModel.Instance;

        gameStateModel.ChargedBankItem = itemConfig;
        JsBridge.Instance.SendCommandToJs("BuyMoney", new BuyVkMoneyPayload(itemConfig.IsGold, itemConfig.Value));

        AnalyticsManager.Instance.SendStoreItemClick(itemConfig.IsGold, itemConfig.Id);
    }
}

public struct BuyVkMoneyPayload
{
    public string product;

    public BuyVkMoneyPayload(bool isGold, int value)
    {
        product = $"{(isGold ? "gold" : "cash")}_{value.ToString()}";
    }
}
