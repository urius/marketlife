using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct UIUpgradePopupBuyClickCommand
    {
        public void Execute(UpgradesPopupItemViewModelBase viewModel)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var shopModel = playerModel.ShopModel;
            var dispatcher = Dispatcher.Instance;
            var gameStateModel = GameStateModel.Instance;
            var analyticsManager = AnalyticsManager.Instance;

            if (viewModel.ItemType == UpgradesPopupItemType.Upgrade)
            {
                var upgradeConfigToBuy = (viewModel as UpgradesPopupUpgradeItemViewModel).UpgradeConfig;
                var isSuccess = false;
                if (playerModel.CanSpendMoney(upgradeConfigToBuy.Price))
                {
                    if (ApplyUpgarde(upgradeConfigToBuy))
                    {
                        (gameStateModel.ShowingPopupModel as UpgradesPopupViewModel).UpdateItems();
                        playerModel.TrySpendMoney(upgradeConfigToBuy.Price);
                        isSuccess = true;
                    }
                }
                else
                {
                    new NotEnoughtMoneySequenceCommand().Execute(upgradeConfigToBuy.Price.IsGold);
                }

                analyticsManager.SendCustom(AnalyticsManager.EventNameApplyUpgrade,
                    ("type", upgradeConfigToBuy.UpgradeTypeStr), ("value", upgradeConfigToBuy.Value), ("success", isSuccess));
            }
            else if (viewModel.ItemType == UpgradesPopupItemType.Personal)
            {
                var personalConfigToBuy = (viewModel as UpgradesPopupPersonalItemViewModel).PersonalConfig;
                var isSuccess = false;
                var price = personalConfigToBuy.GetPrice(shopModel.ShopDesign.Square);
                if (playerModel.CanSpendMoney(price))
                {
                    playerModel.TrySpendMoney(price);
                    shopModel.PersonalModel.SetPersonalWorkingTime(personalConfigToBuy, gameStateModel.ServerTime + personalConfigToBuy.WorkHours * 3600);
                    isSuccess = true;
                }
                else
                {
                    new NotEnoughtMoneySequenceCommand().Execute(price.IsGold);
                }

                analyticsManager.SendCustom(AnalyticsManager.EventNameHirePersonalUpgrade,
                    ("type", personalConfigToBuy.RawIdStr), ("success", isSuccess));
            }
        }

        private bool ApplyUpgarde(UpgradeConfig upgradeConfig)
        {
            var playerModelHolder = PlayerModelHolder.Instance;
            var shopModel = playerModelHolder.ShopModel;

            switch (upgradeConfig.UpgradeType)
            {
                case UpgradeType.ExpandX:
                    if (shopModel.ShopDesign.SizeX < upgradeConfig.Value)
                    {
                        shopModel.ShopDesign.SetSizeX(upgradeConfig.Value);
                        return true;
                    }
                    break;
                case UpgradeType.ExpandY:
                    if (shopModel.ShopDesign.SizeY < upgradeConfig.Value)
                    {
                        shopModel.ShopDesign.SetSizeY(upgradeConfig.Value);
                        return true;
                    }
                    break;
                case UpgradeType.WarehouseSlots:
                    var warehouseSize = shopModel.WarehouseModel.Size;
                    if (warehouseSize < upgradeConfig.Value)
                    {
                        shopModel.WarehouseModel.AddSlots(upgradeConfig.Value - warehouseSize);
                        return true;
                    }
                    break;
                case UpgradeType.WarehouseVolume:
                    var warehouseVolume = shopModel.WarehouseModel.Volume;
                    if (warehouseVolume < upgradeConfig.Value)
                    {
                        shopModel.WarehouseModel.AddVolume(upgradeConfig.Value - warehouseVolume);
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
