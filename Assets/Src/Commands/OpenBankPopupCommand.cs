public struct OpenBankPopupCommand
{
    public async void Execute(bool isGold)
    {
        var gameStateModel = GameStateModel.Instance;
        var bankConfig = BankConfig.Instance;
        var viewModel = new BankPopupViewModel(isGold ? 0 : 1, bankConfig);
        gameStateModel.ShowPopup(viewModel);

        await new LoadBankConfigCommand().ExecuteAsync();
    }
}
