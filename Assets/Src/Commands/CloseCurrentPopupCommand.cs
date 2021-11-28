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
            var advertViewStateModel = AdvertViewStateModel.Instance;

            gameStateModel.RemoveCurrentPopupIfNeeded();

            var isX2AdsViewed = advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2);
            var cashAmountToAdd = offlineReport.SellProfit * (isX2AdsViewed ? 2 : 1);

            warehouseModel.RemoveDeliveredProducts(offlineReport.SoldFromWarehouse, gameStateModel.ServerTime);
            userShopModel.RemoveProducts(offlineReport.SoldFromShelfs);
            playerModel.AddCash(cashAmountToAdd); //TODO Animate cash adding
            playerModel.AddExp(offlineReport.ExpToAdd); //TODO Animate exp adding ?

            playerModel.ApplyExternalActions();
            playerModel.ExternalActionsModel.Clear();

            advertViewStateModel.ResetTarget(AdvertTargetType.OfflineProfitX2);

            gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);

            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameOfflineProfit,
                ("cash", offlineReport.SellProfit), ("exp", offlineReport.ExpToAdd), ("ads_viewed", isX2AdsViewed));
        }
        else
        {
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
    }
}
