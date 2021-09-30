public struct ProcessBonusClickCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var serverTime = gameStateModel.ServerTime;
        var lastBonusTakeTime = playerModel.BonusState.LastBonusTakeTimestamp;
        var analyticsManager = AnalyticsManager.Instance;

        var isCompensation = playerModel.BonusState.IsOldGameBonusProcessed == false;
        if (isCompensation)
        {
            gameStateModel.ShowPopup(new OldGameCompensationPopupViewModel());

            analyticsManager.SendCustom(AnalyticsManager.EventNameOldGameCompensationClick);
        }
        else if (DateTimeHelper.IsSameDays(serverTime, lastBonusTakeTime) == false)
        {
            var bonusConfig = GameConfigManager.Instance.DailyBonusConfig;
            var viewModel = new DailyBonusPopupViewModel(playerModel.BonusState, bonusConfig, serverTime);
            gameStateModel.ShowPopup(viewModel);

            analyticsManager.SendCustom(AnalyticsManager.EventNameDailyBonusClick, ("day_num", viewModel.CurrentBonusDay));
        }
    }
}