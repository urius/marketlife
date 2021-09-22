public struct ProcessBonusClickCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var serverTime = gameStateModel.ServerTime;
        var lastBonusTakeTime = playerModel.BonusState.LastBonusTakeTimestamp;

        if (playerModel.BonusState.IsOldGameBonusProcessed == false)
        {
            gameStateModel.ShowPopup(new OldGameCompensationPopupViewModel());
        }
        else if (DateTimeHelper.IsSameDays(serverTime, lastBonusTakeTime) == false)
        {
            var bonusConfig = GameConfigManager.Instance.DailyBonusConfig;
            gameStateModel.ShowPopup(new DailyBonusPopupViewModel(playerModel.BonusState, bonusConfig, serverTime));
        }
    }
}