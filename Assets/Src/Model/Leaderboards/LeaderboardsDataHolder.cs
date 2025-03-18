using System;
using System.Collections.Generic;

namespace Src.Model.Leaderboards
{
    public class LeaderboardsDataHolder
    {
        public event Action<LeaderboardType> LeaderboardDataSet;
        
        public static readonly LeaderboardsDataHolder Instance = new();

        private readonly Dictionary<LeaderboardType, LeaderboardUserData[]> _leaderboardsData = new();

        public bool IsLeaderboardsSet => _leaderboardsData.Count > 0;

        public void SetLeaderboardData(LeaderboardType type, LeaderboardUserData[] data)
        {
            if (data is { Length: > 0 })
            {
                _leaderboardsData[type] = data;
                LeaderboardDataSet?.Invoke(type);
            }
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
        public readonly string Uid;
        public readonly int Rank;
        public readonly int LeaderboardValue;
        public readonly string DisplayedName;
        public readonly string PictureUrl;

        public LeaderboardUserData(string uid, int rank, int leaderboardValue, string displayedName, string pictureUrl)
        {
            Rank = rank;
            LeaderboardValue = leaderboardValue;
            DisplayedName = displayedName;
            PictureUrl = pictureUrl;
            Uid = uid;
        }
    }
}