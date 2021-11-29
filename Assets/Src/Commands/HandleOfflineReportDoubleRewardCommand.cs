public struct HandleOfflineReportDoubleRewardCommand
{
    public void Execute()
    {
        var advertViewStateModel = AdvertViewStateModel.Instance;
        var dispatcher = Dispatcher.Instance;

        if (advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2) == false)
        {
            advertViewStateModel.PrepareTarget(AdvertTargetType.OfflineProfitX2);
            dispatcher.RequestShowAdvert();
        }
    }
}
