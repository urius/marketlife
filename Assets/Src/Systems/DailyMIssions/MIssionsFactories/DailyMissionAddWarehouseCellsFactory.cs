using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Missions;
using Src.Systems.DailyMIssions.Processors;

namespace Src.Systems.DailyMIssions.MIssionsFactories
{
    public class DailyMissionAddWarehouseCellsFactory : DailyMissionFactoryBase<DailyMissionAddWarehouseCellsProcessor>
    {
        protected override string Key => MissionKeys.AddWarehouseCells;

        public override bool CanAdd()
        {
            return base.CanAdd()
                   && ExtraCanAddConditions();
        }

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var whSize = playerModel.ShopModel.WarehouseModel.Size;
            var nextUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseSlots, whSize);
            if (nextUpgrade != null)
            {
                return new DailyMissionModel(Key, whSize, nextUpgrade.Value, whSize, ChooseReward(complexityMultiplier));
            }
            else
            {
                return null;
            }
        }

        private bool ExtraCanAddConditions()
        {
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var nextUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseSlots, playerModel.ShopModel.WarehouseModel.Size);
            return nextUpgrade != null;
        }
    }
}
