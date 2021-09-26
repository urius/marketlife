public struct HandleExpandShopClickCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var personalConfig = GameConfigManager.Instance.PersonalConfig;
        var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;

        var popupModel = new UpgradesPopupViewModel(playerModel, personalConfig, upgradesConfig, TabType.WarehouseUpgrades);
        gameStateModel.ShowPopup(popupModel);
    }
}
