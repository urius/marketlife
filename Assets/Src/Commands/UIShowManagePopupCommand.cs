public struct UIShowManagePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var personalConfig = GameConfigManager.Instance.PersonalConfig;
        var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;

        gameStateModel.ShowPopup(new UpgradesPopupViewModel(playerModel, personalConfig, upgradesConfig, TabType.ManagePersonal));
    }
}