public struct UIUpgradePopupBuyClickCommand
{
    public void Execute(UpgradesPopupItemViewModelBase viewModel)
    {
        var shopModel = GameStateModel.Instance.PlayerShopModel;
        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;

        if (viewModel.ItemType == UpgradesPopupItemType.Upgrade)
        {
            var upgradeConfigToBuy = (viewModel as UpgradesPopupUpgradeItemViewModel).UpgradeConfig;
            if (shopModel.CanSpendMoney(upgradeConfigToBuy.Price))
            {
                if (ApplyUpgarde(upgradeConfigToBuy))
                {
                    (gameStateModel.ShowingPopupModel as UpgradesPopupViewModel).UpdateItems();
                    shopModel.TrySpendMoney(upgradeConfigToBuy.Price);
                }
            }
            else
            {
                dispatcher.UIRequestBlinkMoney(upgradeConfigToBuy.Price.IsGold);
            }
        }
        else if (viewModel.ItemType == UpgradesPopupItemType.Personal)
        {
            var personalConfigToBuy = (viewModel as UpgradesPopupPersonalItemViewModel).PersonalConfig;
            if (shopModel.CanSpendMoney(personalConfigToBuy.Price))
            {
                shopModel.TrySpendMoney(personalConfigToBuy.Price);
                shopModel.PersonalModel.SetPersonalWorkingTime(personalConfigToBuy.TypeId, gameStateModel.ServerTime + personalConfigToBuy.WorkHours * 3600);
            }
            else
            {
                dispatcher.UIRequestBlinkMoney(personalConfigToBuy.Price.IsGold);
            }
        }
    }

    private bool ApplyUpgarde(UpgradeConfig upgradeConfig)
    {
        var shopModel = GameStateModel.Instance.PlayerShopModel;

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