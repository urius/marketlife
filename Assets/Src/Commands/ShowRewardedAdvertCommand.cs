using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Model;

namespace Src.Commands
{
    public struct ShowRewardedAdvertCommand
    {
        public UniTask<bool> Execute(AdvertTargetType advertTargetType)
        {
            var advertStateModel = AdvertViewStateModel.Instance;
            
            advertStateModel.PrepareTarget(advertTargetType);
            Dispatcher.Instance.RequestShowAdvert();

            return advertStateModel.CurrentShowingAdsTask;
        }
    }
}