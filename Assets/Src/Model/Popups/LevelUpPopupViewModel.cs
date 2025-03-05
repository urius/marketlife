using System.Collections.Generic;
using Src.Model.Configs;

namespace Src.Model.Popups
{
    public class LevelUpPopupViewModel : PopupViewModelBase
    {
        public IReadOnlyList<ProductConfig> NewProducts;
        public IReadOnlyList<ItemConfig<ShelfConfigDto>> NewShelfs;
        public IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> NewFloors;
        public IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> NewWalls;
        public IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> NewWindows;
        public IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> NewDoors;
        public IReadOnlyList<PersonalConfig> NewPersonal;
        public IReadOnlyList<UpgradeConfig> NewUpgrades;

        public LevelUpPopupViewModel(
            IReadOnlyList<ProductConfig> newProducts,
            IReadOnlyList<ItemConfig<ShelfConfigDto>> newShelfs,
            IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> newFloors,
            IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> newWalls,
            IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> newWindows,
            IReadOnlyList<ItemConfig<ShopDecorationConfigDto>> newDoors,
            IReadOnlyList<PersonalConfig> newPersonal,
            IReadOnlyList<UpgradeConfig> newUpgrades)
        {
            NewProducts = newProducts;
            NewShelfs = newShelfs;
            NewFloors = newFloors;
            NewWalls = newWalls;
            NewWindows = newWindows;
            NewDoors = newDoors;
            NewPersonal = newPersonal;
            NewUpgrades = newUpgrades;
        }

        public bool HasNewDecor => NewFloors.Count > 0 || NewWalls.Count > 0 || NewWindows.Count > 0 || NewDoors.Count > 0;
        public override PopupType PopupType => PopupType.LevelUp;
    }
}
