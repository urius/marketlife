public struct UIShowManagePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var personalConfig = GameConfigManager.Instance.PersonalConfig;
        var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;

        var popupModel = (gameStateModel.GetPopupFromCache(PopupType.Upgrades) ?? new UpgradesPopupViewModel(playerModel, personalConfig, upgradesConfig)) as UpgradesPopupViewModel;
        popupModel.UpdateItems();
        gameStateModel.CachePopup(popupModel);
        gameStateModel.ShowPopup(popupModel);
    }
}