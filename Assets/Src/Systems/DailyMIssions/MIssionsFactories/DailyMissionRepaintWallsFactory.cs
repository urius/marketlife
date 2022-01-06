using System.Linq;
using UnityEngine;

public class DailyMissionRepaintWallsFactory : DailyMissionFactoryBase<DailyMissionRepaintWallsProcessor>
{
    protected override string Key => MissionKeys.RepaintWalls;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && ExtraCanAddCondition();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var playerShopModel = PlayerModelHolder.Instance.ShopModel;
        var wallsCount = playerShopModel.ShopDesign.SizeX + playerShopModel.ShopDesign.SizeY;
        var targetValue = (int)Mathf.Max(1, Mathf.Lerp(1, wallsCount, complexityMultiplier));
        var reward = ChooseReward(complexityMultiplier);
        return new DailyMissionModel(Key, 0, targetValue, 0, reward);
    }

    private bool ExtraCanAddCondition()
    {
        var wallsConfig = GameConfigManager.Instance.WallsConfig;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var wallsCountOnLevel = wallsConfig.GetWallsConfigsForLevel(playerModel.ProgressModel.Level).Count();
        return wallsCountOnLevel > 1;
    }
}
