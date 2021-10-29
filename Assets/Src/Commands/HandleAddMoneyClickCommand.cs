using Cysharp.Threading.Tasks;

public struct HandleAddMoneyClickCommand
{
    public async UniTaskVoid Execute(bool isGold)
    {
        var gameStateModel = GameStateModel.Instance;
        var bankConfig = BankConfig.Instance;
        var viewModel = new BankPopupViewModel(isGold ? 0 : 1, bankConfig);
        gameStateModel.ShowPopup(viewModel);

        AnalyticsManager.Instance.SendStoreOpened(isGold);

        await new LoadBankConfigCommand().ExecuteAsync();
    }
}
