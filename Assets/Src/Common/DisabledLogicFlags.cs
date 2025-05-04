namespace Src.Common
{
    public static class DisabledLogicFlags
    {
        public static bool IsFriendsLogicDisabled => MirraSdkWrapper.IsYandexGames || MirraSdkWrapper.IsCrazyGames;
        public static bool IsServerDataDisabled => MirraSdkWrapper.IsYandexGames;
        public static bool IsLeaderboardsDisabled => false;
    }
}