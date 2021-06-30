using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradesPopupViewModel : PopupViewModelBase
{
    public event Action ItemsUpdated = delegate { };

    private const int MaxTabsCount = 3;

    public readonly TabType ShowOnTab;
    public readonly IDictionary<TabType, UpgradesPopupItemViewModelBase[]> ItemViewModelsByTabKey = new Dictionary<TabType, UpgradesPopupItemViewModelBase[]>(MaxTabsCount);

    private readonly UserModel _playerModel;
    private readonly ShopModel _shopModel;
    private readonly IPersonalConfig _personalConfig;
    private readonly IUpgradesConfig _upgradesConfig;
    private readonly List<TabType> _tabs = new List<TabType>(MaxTabsCount);

    public UpgradesPopupViewModel(
        UserModel playerModel,
        IPersonalConfig personalConfig,
        IUpgradesConfig upgradesConfig,
        TabType showOnTab = TabType.Undefined)
    {
        _playerModel = playerModel;
        _shopModel = playerModel.ShopModel;
        _personalConfig = personalConfig;
        _upgradesConfig = upgradesConfig;
        ShowOnTab = showOnTab;

        UpdateItems();
    }

    public IReadOnlyList<TabType> TabKeys => _tabs;
    public override PopupType PopupType => PopupType.Upgrades;

    public int GetTabIndex(TabType tabType)
    {
        for (var i = 0; i < TabKeys.Count; i++)
        {
            if (TabKeys[i] == tabType)
            {
                return i;
            }
        }
        return -1;
    }

    public void UpdateItems()
    {
        _tabs.Clear();
        ItemViewModelsByTabKey.Clear();

        _tabs.Add(TabType.ExpandUpgrades);
        ItemViewModelsByTabKey[TabType.ExpandUpgrades] = new UpgradesPopupItemViewModelBase[]{
            GetUpgradeViewModel(UpgradeType.ExpandX, _shopModel.ShopDesign.SizeX),
            GetUpgradeViewModel(UpgradeType.ExpandY, _shopModel.ShopDesign.SizeY),
        };

        _tabs.Add(TabType.WarehouseUpgrades);
        ItemViewModelsByTabKey[TabType.WarehouseUpgrades] = new UpgradesPopupItemViewModelBase[]{
            GetUpgradeViewModel(UpgradeType.WarehouseSlots, _shopModel.WarehouseModel.Size),
            GetUpgradeViewModel(UpgradeType.WarehouseVolume, _shopModel.WarehouseModel.Volume),
        };

        var personalConfigs = _personalConfig.GetPersonalConfigsForLevel(_playerModel.ProgressModel.Level);
        if (personalConfigs.Any())
        {
            _tabs.Add(TabType.ManagePersonal);
            ItemViewModelsByTabKey[TabType.ManagePersonal] = personalConfigs.Select(c => new UpgradesPopupPersonalItemViewModel(c)).ToArray();
        }

        ItemsUpdated();
    }

    private UpgradesPopupUpgradeItemViewModel GetUpgradeViewModel(UpgradeType upgradeType, int value)
    {
        var nextUpgrade = _upgradesConfig.GetNextUpgradeForValue(upgradeType, value);
        if (nextUpgrade != null)
        {
            return new UpgradesPopupUpgradeItemViewModel(nextUpgrade);
        }
        else
        {
            var currentUpgrade = _upgradesConfig.GetCurrentUpgradeForValue(upgradeType, value);
            return new UpgradesPopupUpgradeItemViewModel(currentUpgrade, isMaxReached: true);
        }
    }
}

public enum TabType
{
    Undefined,
    ExpandUpgrades,
    WarehouseUpgrades,
    ManagePersonal,
}

public abstract class UpgradesPopupItemViewModelBase
{
    public abstract UpgradesPopupItemType ItemType { get; }
}

public class UpgradesPopupUpgradeItemViewModel : UpgradesPopupItemViewModelBase
{
    public readonly UpgradeConfig UpgradeConfig;
    public readonly bool IsMaxReached;

    public UpgradesPopupUpgradeItemViewModel(UpgradeConfig upgradeConfig, bool isMaxReached = false)
    {
        UpgradeConfig = upgradeConfig;
        IsMaxReached = isMaxReached;
    }

    public override UpgradesPopupItemType ItemType => UpgradesPopupItemType.Upgrade;
}

public class UpgradesPopupPersonalItemViewModel : UpgradesPopupItemViewModelBase
{
    public readonly PersonalConfig PersonalConfig;

    public UpgradesPopupPersonalItemViewModel(PersonalConfig personalConfig)
    {
        PersonalConfig = personalConfig;
    }

    public override UpgradesPopupItemType ItemType => UpgradesPopupItemType.Personal;
}

public enum UpgradesPopupItemType
{
    Undefined,
    Upgrade,
    Personal,
}
