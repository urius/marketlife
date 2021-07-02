public struct CalculateForReportCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var userShopModel = playerModel.ShopModel;
        var warehouseModel = userShopModel.WarehouseModel;
        var playerOfflineReportHolder = PlayerOfflineReportHolder.Instance;

        var (soldFromShelfsProducts, soldFromWarehouseProducts) = playerModel.CalculateSellsToTime(gameStateModel.ServerTime);

        warehouseModel.RemoveUndeliveredProducts(soldFromWarehouseProducts, gameStateModel.ServerTime);//TODO move to close report popup
        userShopModel.RemoveProducts(soldFromShelfsProducts);

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

        gameStateModel.SetGameState(GameStateName.ShopSimulation);
    }
}
