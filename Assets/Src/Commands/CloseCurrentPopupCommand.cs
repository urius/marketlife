public struct CloseCurrentPopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;

        if (gameStateModel.GameState == GameStateName.ReadyForStart
            && gameStateModel.ShowingPopupModel.PopupType == PopupType.OfflineReport)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var userShopModel = playerModel.ShopModel;
            var warehouseModel = userShopModel.WarehouseModel;
            var offlineReport = PlayerOfflineReportHolder.Instance.PlayerOfflineReport;

            gameStateModel.SetGameState(GameStateName.ShopSimulation);

            warehouseModel.RemoveUndeliveredProducts(offlineReport.SoldFromWarehouse, gameStateModel.ServerTime);
            userShopModel.RemoveProducts(offlineReport.SoldFromShelfs);
            playerModel.AddCash(offlineReport.SellProfit); //TODO Animate cash adding
        }

        gameStateModel.RemoveCurrentPopupIfNeeded();
    }
}
