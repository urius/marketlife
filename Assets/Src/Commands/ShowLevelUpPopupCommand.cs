using System.Collections.Generic;
using System.Linq;

public class ShowLevelUpPopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var progressModel = playerModel.ProgressModel;
        var prevLevel = progressModel.PreviousLevel;
        var currentLevel = progressModel.Level;
        var newProducts = GetNewProductConfigs(prevLevel, currentLevel);
        var newShelfs = GetNewShelfsConfigs(prevLevel, currentLevel);
        var newFloors = GetNewFloorsConfigs(prevLevel, currentLevel);
        var newWalls = GetNewWallsConfigs(prevLevel, currentLevel);
        var newWindows = GetNewWindowsConfigs(prevLevel, currentLevel);
        var newDoors = GetNewDoorsConfigs(prevLevel, currentLevel);
        var newPersonal = GetNewPersonalConfigs(prevLevel, currentLevel);
        var newUpgrades = GetNewUpgradesConfigs(prevLevel, currentLevel);

        var popupViewModel = new LevelUpPopupViewModel(newProducts, newShelfs, newFloors, newWalls, newWindows, newDoors, newPersonal, newUpgrades);
        gameStateModel.ShowPopup(popupViewModel);
    }

    private IReadOnlyList<ProductConfig> GetNewProductConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.ProductsConfig;
        var configsForCurrentLevel = configs.GetProductConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<ItemConfig<ShelfConfigDto>> GetNewShelfsConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.ShelfsConfig;
        var configsForCurrentLevel = configs.GetShelfConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> GetNewFloorsConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.FloorsConfig;
        var configsForCurrentLevel = configs.GetFloorsConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> GetNewWallsConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.WallsConfig;
        var configsForCurrentLevel = configs.GetWallsConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> GetNewWindowsConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.WindowsConfig;
        var configsForCurrentLevel = configs.GetWindowsConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> GetNewDoorsConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.DoorsConfig;
        var configsForCurrentLevel = configs.GetDoorsConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<PersonalConfig> GetNewPersonalConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.PersonalConfig;
        var configsForCurrentLevel = configs.GetPersonalConfigsForLevel(currentLevel);
        return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
    }

    private IReadOnlyList<UpgradeConfig> GetNewUpgradesConfigs(int previousLevel, int currentLevel)
    {
        var configs = GameConfigManager.Instance.UpgradesConfig;
        var expandXUpgrades = configs.GetAllUpgradesBytype(UpgradeType.ExpandX)
            .Concat(configs.GetAllUpgradesBytype(UpgradeType.ExpandY))
            .Concat(configs.GetAllUpgradesBytype(UpgradeType.WarehouseSlots))
            .Concat(configs.GetAllUpgradesBytype(UpgradeType.WarehouseVolume))
            .Where(c => c.UnlockLevel > previousLevel && c.UnlockLevel <= currentLevel);
        return expandXUpgrades.ToArray();
    }

    private IReadOnlyList<T> FilterConfigsByMinLevel<T>(IEnumerable<T> configsForCurrentLevel, int minLevel)
        where T : IUnlockableConfig
    {
        var resultConfigs = new List<T>(configsForCurrentLevel.Count());
        foreach (var config in configsForCurrentLevel)
        {
            if (config.UnlockLevel >= minLevel)
            {
                resultConfigs.Add(config);
            }
        }
        return resultConfigs;
    }
}
