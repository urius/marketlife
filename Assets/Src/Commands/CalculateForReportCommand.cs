public struct CalculateForReportCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerOfflineReportHolder = PlayerOfflineReportHolder.Instance;

        var calculationResult = playerModel.CalculateOfflineToTime(gameStateModel.ServerTime);

        for (var i = 0; i < calculationResult.UnwashesAddedAmount; i++)
        {
            playerModel.ShopModel.AddRandomUnwash();
        }

        var report = new UserOfflineReportModel(
            playerModel.StatsData.LastVisitTimestamp,
            gameStateModel.ServerTime,
            calculationResult.SoldFromShelfs,
            calculationResult.SoldFromWarehouse,
            calculationResult.GrabbedProducts,
            calculationResult.UnwashesCleanedAmount);
        playerOfflineReportHolder.SetReport(report);

        if (report.IsEmpty == false)
        {
            gameStateModel.ShowPopup(new OfflineReportPopupViewModel(report));
        }
        else
        {
            gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);
        }
    }
}
