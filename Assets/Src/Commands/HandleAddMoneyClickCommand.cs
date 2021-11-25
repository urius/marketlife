public struct HandleAddMoneyClickCommand
{
    public void Execute(bool isGold)
    {
        new OpenBankPopupCommand().Execute(isGold);

        AnalyticsManager.Instance.SendStoreOpened(isGold);
    }
}
