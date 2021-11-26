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

            var isAdsViewed = advertViewStateModel.IsRewardCharged;
            var cashAmountToAdd = offlineReport.SellProfit + (isAdsViewed ? advertViewStateModel.Reward.Value : 0);

            warehouseModel.RemoveDeliveredProducts(offlineReport.SoldFromWarehouse, gameStateModel.ServerTime);
            userShopModel.RemoveProducts(offlineReport.SoldFromShelfs);
            playerModel.AddCash(cashAmountToAdd); //TODO Animate cash adding
            playerModel.AddExp(offlineReport.ExpToAdd); //TODO Animate exp adding ?

            playerModel.ApplyExternalActions();
            playerModel.ExternalActionsModel.Clear();

            advertViewStateModel.ResetChargedReward();

            gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);

            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameOfflineProfit,
                ("cash", offlineReport.SellProfit), ("exp", offlineReport.ExpToAdd), ("ads_viewed", isAdsViewed));
        }
        else
        {
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
    }
}
