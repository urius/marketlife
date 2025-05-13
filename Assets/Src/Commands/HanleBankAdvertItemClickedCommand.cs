using Src.Common;
using Src.Managers;
using Src.Model;
using UnityEngine;

namespace Src.Commands
{
    public struct HanleBankAdvertItemClickedCommand
    {
        public void Execute(bool isGold)
        {
            var advertStateModel = AdvertViewStateModel.Instance;
            var advertConfig = GameConfigManager.Instance.AdvertConfig;

            var restWatchesCount = Mathf.Max(0, advertConfig.AdvertDefaultWatchesCount - advertStateModel.BankAdvertWatchesCount);
            if (restWatchesCount > 0)
            {
                AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventBankAdsViewClick, ("is_gold", isGold));

                new ShowRewardedAdvertCommand().Execute(isGold ? AdvertTargetType.BankGold : AdvertTargetType.BankCash);
            }
        }
    }
}
