using System.Collections.Generic;
using System.Linq;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct ProcessLevelUpCommand
    {
        public void Execute()
        {
            var gameStateModel = GameStateModel.Instance;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var progressModel = playerModel.ProgressModel;
            var prevLevel = progressModel.PreviousLevel;
            var currentLevel = progressModel.Level;

            AnalyticsManager.Instance.SendLevelUp(currentLevel);

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

            new ActualizePlayerDataCommand().Execute();
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
            var configs = GameConfigManager.Instance.PersonalsConfig;
            var configsForCurrentLevel = configs.GetPersonalConfigsForLevel(currentLevel);
            return FilterConfigsByMinLevel(configsForCurrentLevel, previousLevel + 1);
        }

        private IReadOnlyList<UpgradeConfig> GetNewUpgradesConfigs(int previousLevel, int currentLevel)
        {
            var configs = GameConfigManager.Instance.UpgradesConfig;
            var expandXUpgrades = GetMaxLevelUPgradeConfigForLEvels(configs.GetAllUpgradesByType(UpgradeType.ExpandX), previousLevel, currentLevel);
            var expandYUpgrades = GetMaxLevelUPgradeConfigForLEvels(configs.GetAllUpgradesByType(UpgradeType.ExpandY), previousLevel, currentLevel);
            var warehouseSlotsUpgrades = GetMaxLevelUPgradeConfigForLEvels(configs.GetAllUpgradesByType(UpgradeType.WarehouseSlots), previousLevel, currentLevel);
            var warehouseVolumeUpgrades = GetMaxLevelUPgradeConfigForLEvels(configs.GetAllUpgradesByType(UpgradeType.WarehouseVolume), previousLevel, currentLevel);

            var result = new List<UpgradeConfig>(4);
            if (expandXUpgrades != null)
            {
                result.Add(expandXUpgrades);
            }
            if (expandYUpgrades != null)
            {
                result.Add(expandYUpgrades);
            }
            if (warehouseSlotsUpgrades != null)
            {
                result.Add(warehouseSlotsUpgrades);
            }
            if (warehouseVolumeUpgrades != null)
            {
                result.Add(warehouseVolumeUpgrades);
            }
            return result;
        }

        private UpgradeConfig GetMaxLevelUPgradeConfigForLEvels(IEnumerable<UpgradeConfig> configs, int previousLevel, int currentLevel)
        {
            var configsForLevels = configs.Where(c => c.UnlockLevel > previousLevel && c.UnlockLevel <= currentLevel);
            var result = configsForLevels.FirstOrDefault();
            if (result != null)
            {
                foreach (var config in configsForLevels)
                {
                    if (config.UnlockLevel > result.UnlockLevel)
                    {
                        result = config;
                    }
                }
            }
            return result;
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
}
