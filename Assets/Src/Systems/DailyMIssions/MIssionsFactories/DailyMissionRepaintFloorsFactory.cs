using System.Linq;
using UnityEngine;

public class DailyMissionRepaintFloorsFactory : DailyMissionFactoryBase<DailyMissionRepaintFloorsProcessor>
{
    protected override string Key => MissionKeys.RepaintFloors;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && ExtraCanAddCondition();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var playerShopModel = PlayerModelHolder.Instance.ShopModel;
        var floorsCount = playerShopModel.ShopDesign.Square;
        var targetValue = (int)Mathf.Max(1, Mathf.Lerp(1, floorsCount, complexityMultiplier));
        var reward = ChooseReward(complexityMultiplier);
        return new DailyMissionModel(Key, 0, targetValue, 0, reward);
    }

    private bool ExtraCanAddCondition()
    {
        var floorsConfig = GameConfigManager.Instance.FloorsConfig;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var floorsCountOnLevel = floorsConfig.GetFloorsConfigsForLevel(playerModel.ProgressModel.Level).Count();
        return floorsCountOnLevel > 1;
    }
}
