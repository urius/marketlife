using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Missions;
using Src.Systems.DailyMission.Processors;

namespace Src.Systems.DailyMission.MissionFactories
{
    public class DailyMissionUpgradeWarehouseVolumeFactory : DailyMissionFactoryBase<DailyMissionUpgradeWarehouseVolumeProcessor>
    {
        protected override string Key => MissionKeys.UpgradeWarehouseVolume;

        public override bool CanAdd()
        {
            return base.CanAdd()
                   && ExtraCanAddConditions();
        }

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var whVolume = playerModel.ShopModel.WarehouseModel.Volume;
            var nextUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseVolume, whVolume);
            if (nextUpgrade != null)
            {
                return new DailyMissionModel(Key, whVolume, nextUpgrade.Value, whVolume, ChooseReward(complexityMultiplier));
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
            var nextUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseVolume, playerModel.ShopModel.WarehouseModel.Volume);
            return nextUpgrade != null;
        }
    }
}
