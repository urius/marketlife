using Src.Common;
using Src.Model;

namespace Src.Commands
{
    public struct HandleOfflineReportDoubleRewardCommand
    {
        public void Execute()
        {
            var advertViewStateModel = AdvertViewStateModel.Instance;

            if (advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2) == false)
            {
                new ShowRewardedAdvertCommand().Execute(AdvertTargetType.OfflineProfitX2);
            }
            else if (advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2) == false)
            {
                new ShowRewardedAdvertCommand().Execute(AdvertTargetType.OfflineExpX2);
            }
        }
    }
}
