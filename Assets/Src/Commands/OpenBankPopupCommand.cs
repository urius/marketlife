public struct OpenBankPopupCommand
{
    public async void Execute(bool isGold)
    {
        var gameStateModel = GameStateModel.Instance;
        var bankConfig = BankConfig.Instance;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var viewModel = new BankPopupViewModel(isGold ? 0 : 1, bankConfig, mainConfig);
        gameStateModel.ShowPopup(viewModel);

        await new LoadBankConfigCommand().ExecuteAsync();
    }
}