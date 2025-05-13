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

            if (advertStateModel.IsWatched(AdvertTargetType.DailyBonusRewardX2) == false)
            {
                var watchAdsResult = await new ShowRewardedAdvertCommand().Execute(AdvertTargetType.DailyBonusRewardX2);
                
                if (watchAdsResult)
                {
                    new HandleTakeDailyBonusCommand().Execute(itemsWorldPositions);
                }
            }
        }
    }
}
