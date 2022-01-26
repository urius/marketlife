public class LeaderboardsPopupViewModel : PopupViewModelBase
{
    public readonly LeaderboardUserData[] ExpLBData;
    public readonly LeaderboardUserData[] FriendsLBData;
    public readonly LeaderboardUserData[] CashLBData;
    public readonly LeaderboardUserData[] GoldLBData;

    public LeaderboardsPopupViewModel(
        LeaderboardUserData[] expLBData,
        LeaderboardUserData[] friendsLBData,
        LeaderboardUserData[] cashLBData,
        LeaderboardUserData[] goldLBData)
    {
        ExpLBData = expLBData;
        FriendsLBData = friendsLBData;
        CashLBData = cashLBData;
        GoldLBData = goldLBData;
    }

    public override PopupType PopupType => PopupType.Leaderboards;
}
