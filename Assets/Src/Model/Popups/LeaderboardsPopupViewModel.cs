namespace Src.Model.Popups
{
    public class LeaderboardsPopupViewModel : PopupViewModelBase
    {
        public readonly LeaderboardUserData[] ExpLBData;
        public readonly LeaderboardUserData[] CashLBData;
        public readonly LeaderboardUserData[] GoldLBData;
        public readonly LeaderboardUserData[] FriendsLBData;

        public LeaderboardsPopupViewModel(
            LeaderboardUserData[] expLBData,
            LeaderboardUserData[] cashLBData,
            LeaderboardUserData[] goldLBData,
            LeaderboardUserData[] friendsLBData)
        {
            ExpLBData = expLBData;
            CashLBData = cashLBData;
            GoldLBData = goldLBData;
            FriendsLBData = friendsLBData;
        }

        public override PopupType PopupType => PopupType.Leaderboards;
    }
}
