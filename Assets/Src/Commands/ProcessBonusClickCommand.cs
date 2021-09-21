public struct ProcessBonusClickCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;

        if (playerModel.BonusState.IsOldGameBonusProcessed == false)
        {
            gameStateModel.ShowPopup(new OldGameCompensationPopupViewModel());
        }
    }
}