using Src.Common;
using Src.Model;
using UnityEngine;

namespace Src.Commands
{
    public struct HandleDailyBonusDoubleRewardCommand
    {
        public async void Execute(Vector3[] itemsWorldPositions)
        {
            var advertStateModel = AdvertViewStateModel.Instance;
            var dispatcher = Dispatcher.Instance;

            if (advertStateModel.IsWatched(AdvertTargetType.DailyBonus) == false)
            {
                advertStateModel.PrepareTarget(AdvertTargetType.DailyBonus);
                dispatcher.RequestShowAdvert();

                var watchAdsResult = await advertStateModel.CurrentShowingAdsTask;
                if (watchAdsResult)
                {
                    new HandleTakeDailyBonusCommand().Execute(itemsWorldPositions);
                }
            }
        }
    }
}
