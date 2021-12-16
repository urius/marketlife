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

            var isX2ProfitAdsViewed = advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2);
            var isX2ExpAdsViewed = advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2);
            var cashAmountToAdd = offlineReport.SellProfit * (isX2ProfitAdsViewed ? 2 : 1);
            var expAmountToAdd = offlineReport.ExpFromSell * (isX2ExpAdsViewed ? 2 : 1);

            warehouseModel.RemoveDeliveredProducts(offlineReport.SoldFromWarehouse, gameStateModel.ServerTime);
            userShopModel.RemoveProducts(offlineReport.SoldFromShelfs);
            playerModel.AddCash(cashAmountToAdd); //TODO Animate cash adding
            playerModel.AddExp(expAmountToAdd); //TODO Animate exp adding ?

            playerModel.ApplyExternalActions();
            playerModel.ExternalActionsModel.Clear();

            advertViewStateModel.ResetTarget(AdvertTargetType.OfflineProfitX2);

            gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);

            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameOfflineProfit,
                ("cash", offlineReport.SellProfit), ("exp", offlineReport.ExpFromSell), ("ads_viewed", isX2ProfitAdsViewed));
        }
        else
        {
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
    }
}
