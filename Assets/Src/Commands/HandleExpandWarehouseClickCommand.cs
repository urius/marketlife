using Src.Managers;
using Src.Model;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct HandleExpandWarehouseClickCommand
    {
        public void Execute()
        {
            var gameStateModel = GameStateModel.Instance;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var personalConfig = GameConfigManager.Instance.PersonalsConfig;
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var analyticsManager = AnalyticsManager.Instance;

            analyticsManager.SendCustom(AnalyticsManager.EventNameExpandWarehouseClick);
            var popupModel = new UpgradesPopupViewModel(playerModel, personalConfig, upgradesConfig, TabType.WarehouseUpgrades);
            gameStateModel.ShowPopup(popupModel);
        }
    }
}
