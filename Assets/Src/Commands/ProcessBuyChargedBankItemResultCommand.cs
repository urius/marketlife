using Src.Model;

namespace Src.Commands
{
    public struct ProcessBuyChargedBankItemResultCommand
    {
        public void Execute(bool isBuySuccess)
        {
            var gameStateModel = GameStateModel.Instance;
            var playerModelHolder = PlayerModelHolder.Instance;
            
            if (isBuySuccess)
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

            gameStateModel.ChargedBankItem = null;
        }
    }
}