public struct UIShowManagePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.PlayerShopModel;
        var personalConfig = GameConfigManager.Instance.PersonalConfig;
        var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;

        gameStateModel.ShowPopup(new UpgradesPopupViewModel(shopModel, personalConfig, upgradesConfig, TabType.ManagePersonal));
    }
}