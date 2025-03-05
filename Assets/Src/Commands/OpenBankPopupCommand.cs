using Src.Commands.LoadSave;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct OpenBankPopupCommand
    {
        public async void Execute(bool isGold)
        {
            var gameStateModel = GameStateModel.Instance;
            var bankConfig = BankConfig.Instance;
            var mainConfig = GameConfigManager.Instance.MainConfig;
            var playerModelHolder = PlayerModelHolder.Instance;
            var advertStateModel = AdvertViewStateModel.Instance;
            var viewModel = new BankPopupViewModel(
                isGold ? 0 : 1,
                bankConfig,
                mainConfig,
                playerModelHolder.IsBuyInBankAllowed,
                advertStateModel);
            gameStateModel.ShowPopup(viewModel);

            await new LoadBankConfigCommand().ExecuteAsync();
        }
    }
}
