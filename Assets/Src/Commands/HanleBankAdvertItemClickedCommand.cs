using Src.Common;
using Src.Model;
using UnityEngine;

public struct HanleBankAdvertItemClickedCommand
{
    public void Execute(bool isGold)
    {
        var advertStateModel = AdvertViewStateModel.Instance;
        var dispatcher = Dispatcher.Instance;
        var advertConfig = GameConfigManager.Instance.AdvertConfig;

        var restWatchesCount = Mathf.Max(0, advertConfig.AdvertDefaultWatchesCount - advertStateModel.BankAdvertWatchesCount);
        if (restWatchesCount > 0)
        {
            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventBankAdsViewClick, ("is_gold", isGold));
            advertStateModel.PrepareTarget(isGold ? AdvertTargetType.BankGold : AdvertTargetType.BankCash);
            dispatcher.RequestShowAdvert();
        }
    }
}
