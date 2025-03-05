using Src.Common;
using Src.Model;

namespace Src.Commands
{
    public struct HandleDailyBonusDoubleRewardCommand
    {
        public void Execute(int rewardIndex)
        {
            var advertStateModel = AdvertViewStateModel.Instance;
            var dayNum = rewardIndex + 1;
            var advertTarget = advertStateModel.GetAdvertTargetTypeByDailyBonusDayNum(dayNum);
            var dispatcher = Dispatcher.Instance;

            if (advertTarget != AdvertTargetType.None
                && advertStateModel.IsWatched(advertTarget) == false)
            {
                advertStateModel.PrepareTarget(advertTarget);
                dispatcher.RequestShowAdvert();
            }
        }
    }
}
