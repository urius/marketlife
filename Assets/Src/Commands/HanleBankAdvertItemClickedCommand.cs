public struct HanleBankAdvertItemClickedCommand
{
    public void Execute(bool isGold)
    {
        var advertStateModel = AdvertViewStateModel.Instance;
        var dispatcher = Dispatcher.Instance;

        AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventBankAdsViewClick, ("is_gold", isGold));

        advertStateModel.PrepareTarget(isGold ? AdvertTargetType.BankGold : AdvertTargetType.BankCash);
        dispatcher.RequestShowAdvert();
    }
}
