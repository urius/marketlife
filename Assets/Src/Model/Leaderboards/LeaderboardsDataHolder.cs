using System.Collections.Generic;

namespace Src.Model.Leaderboards
{
    public class LeaderboardsDataHolder
    {
        public static LeaderboardsDataHolder Instance = new LeaderboardsDataHolder();

        private Dictionary<LeaderboardType, LeaderboardUserData[]> _leaderboardsData = new Dictionary<LeaderboardType, LeaderboardUserData[]>();

        public bool IsLeaderboardsSet => _leaderboardsData.Count > 0;

        public void SetLeaderboardData(LeaderboardType type, LeaderboardUserData[] data)
        {
            _leaderboardsData[type] = data;
        }

        public LeaderboardUserData[] GetLeaderboardData(LeaderboardType type)
        {
            _leaderboardsData.TryGetValue(type, out var result);
            return result;
        }
    }

    public enum LeaderboardType
    {
        None,
        Exp,
        Friends,
        Cash,
        Gold,
    }

    public class LeaderboardUserData
    {
        public readonly int Rank;
        public readonly int LeaderboardValue;
        public readonly UserSocialData UserSocialData;

        public LeaderboardUserData(int rank, int leaderboardValue, UserSocialData userSocialData)
        {
            Rank = rank;
            LeaderboardValue = leaderboardValue;
            UserSocialData = userSocialData;
        }
    }
}