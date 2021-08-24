public struct BottomPanelHandleBackClickCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;
        switch (gameStateModel.GameState)
        {
            case GameStateName.ShopInterior:
                gameStateModel.SetGameState(GameStateName.ShopSimulation);
                break;
            case GameStateName.ShopFriend:
                gameStateModel.SetViewingUserModel(playerModelHolder.UserModel);
                gameStateModel.SetGameState(GameStateName.ShopSimulation);
                break;
        }
    }
}
