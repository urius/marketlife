using Src.Common;
using Src.Model;

namespace Src.Commands
{
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
            else if (advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2) == false)
            {
                advertViewStateModel.PrepareTarget(AdvertTargetType.OfflineExpX2);
                dispatcher.RequestShowAdvert();
            }
        }
    }
}
