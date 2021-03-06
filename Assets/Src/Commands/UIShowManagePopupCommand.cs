public struct UIShowManagePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var personalConfig = GameConfigManager.Instance.PersonalsConfig;
        var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
        var analyticsManager = AnalyticsManager.Instance;

        analyticsManager.SendCustom(AnalyticsManager.EventNameManageClick);
        var popupModel = (gameStateModel.GetPopupFromCache(PopupType.Upgrades) ?? new UpgradesPopupViewModel(playerModel, personalConfig, upgradesConfig)) as UpgradesPopupViewModel;
        popupModel.UpdateItems();
        gameStateModel.CachePopup(popupModel);
        gameStateModel.ShowPopup(popupModel);
    }
}