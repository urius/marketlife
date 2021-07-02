public struct CalculateForReportCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerOfflineReportHolder = PlayerOfflineReportHolder.Instance;

        var (soldFromShelfsProducts, soldFromWarehouseProducts) = playerModel.CalculateSellsToTime(gameStateModel.ServerTime);

        var report = new UserOfflineReportModel(
            playerModel.StatsData.LastVisitTimestamp,
            gameStateModel.ServerTime,
            soldFromShelfsProducts,
            soldFromWarehouseProducts);
        playerOfflineReportHolder.SetReport(report);

        if (report.IsEmpty == false)
        {
            gameStateModel.ShowPopup(new OfflineReportPopupViewModel(report));
        }
        else
        {
            gameStateModel.SetGameState(GameStateName.ShopSimulation);
        }

    }
}
