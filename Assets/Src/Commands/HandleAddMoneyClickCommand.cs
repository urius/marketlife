using Cysharp.Threading.Tasks;

public struct HandleAddMoneyClickCommand
{
    public async UniTaskVoid Execute(bool isGold)
    {
        var gameStateModel = GameStateModel.Instance;
        var viewModel = new BankPopupViewModel(isGold ? 0 : 1);
        gameStateModel.ShowPopup(viewModel);

        await new LoadBankConfigCommand().ExecuteAsync();
    }
}
