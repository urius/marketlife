public class DailyMissionChangeBillboardFactory : DailyMissionFactoryBase<DailyMissionChangeBillboardProcessor>
{
    protected override string Key => MissionKeys.ChangeBillboard;

    public override bool CanAdd()
    {
        var playerShopModel = PlayerModelHolder.Instance.ShopModel;        
        return base.CanAdd()
            && playerShopModel.BillboardModel.IsAvailable;
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        return new DailyMissionModel(Key, 0, 1, 0, ChooseReward(complexityMultiplier));
    }
}
