using Src.Model;

namespace Src.Commands
{
    public struct BottomPanelHandleBackClickCommand
    {
        public void Execute()
        {
            var gameStateModel = GameStateModel.Instance;
            var playerModelHolder = PlayerModelHolder.Instance;
            switch (gameStateModel.GameState)
            {
                case GameStateName.PlayerShopInterior:
                    gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);
                    break;
                case GameStateName.ShopFriend:
                    gameStateModel.SetViewingUserModel(playerModelHolder.UserModel);
                    gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);
                    break;
            }
        }
    }
}
