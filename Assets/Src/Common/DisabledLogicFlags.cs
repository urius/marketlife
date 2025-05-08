namespace Src.Common
{
    public static class DisabledLogicFlags
    {
        public static bool IsFriendsLogicDisabled => MirraSdkWrapper.IsYandexGames || MirraSdkWrapper.IsCrazyGames;
        public static bool IsSharingDisabled => MirraSdkWrapper.IsYandexGames || MirraSdkWrapper.IsCrazyGames;
        public static bool IsServerDataDisabled => MirraSdkWrapper.IsYandexGames;
        public static bool IsLeaderboardsDisabled => !IsLeaderboardsEnabled;
        public static bool IsPlatformLeaderboardDataDisabled => !IsPlatformLeaderboardDataUsed;
        
        private static bool IsPlatformLeaderboardDataUsed => IsLeaderboardsEnabled && MirraSdkWrapper.IsYandexGames;
        private static bool IsLeaderboardsEnabled => true;
    }
}