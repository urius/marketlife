using System;
using System.Linq;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Missions;
using Src.Systems.DailyMission.Processors;
using UnityEngine;

namespace Src.Systems.DailyMission.MissionFactories
{
    public class DailyMissionExpandShopFactory : DailyMissionFactoryBase<DailyMissionExpandShopProcessor>
    {
        protected override string Key => MissionKeys.ExpandShop;

        public override bool CanAdd()
        {
            return base.CanAdd()
                   && ExtraCanAddCondition();
        }

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var playerLevel = playerModel.ProgressModel.Level;
            var playerShopModel = playerModel.ShopModel;
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var expandXUpgrades = upgradesConfig.GetAllUpgradesByType(UpgradeType.ExpandX);
            var expandYUpgrades = upgradesConfig.GetAllUpgradesByType(UpgradeType.ExpandY);
            var availableExpandsCount = expandXUpgrades.Count(u => u.Value > playerShopModel.ShopDesign.SizeX && u.UnlockLevel <= playerLevel)
                                        + expandXUpgrades.Count(u => u.Value > playerShopModel.ShopDesign.SizeY && u.UnlockLevel <= playerLevel);
            if (availableExpandsCount > 0)
            {
                var targetValue = (int)Math.Max(1, Mathf.Lerp(1, availableExpandsCount, complexityMultiplier));
                var reward = ChooseReward(complexityMultiplier);
                return new DailyMissionModel(Key, 0, targetValue, 0, reward);
            }
            else
            {
                return null;
            }
        }

        private bool ExtraCanAddCondition()
        {
            var upgradesConfig = GameConfigManager.Instance.UpgradesConfig;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var level = playerModel.ProgressModel.Level;
            var playerShopModel = playerModel.ShopModel;
            var nextXUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.ExpandX, playerShopModel.ShopDesign.SizeX);
            var nextYUpgrade = upgradesConfig.GetNextUpgradeForValue(UpgradeType.ExpandY, playerShopModel.ShopDesign.SizeY);

            if ((nextXUpgrade != null && nextXUpgrade.UnlockLevel <= level)
                || (nextYUpgrade != null && nextYUpgrade.UnlockLevel <= level))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
