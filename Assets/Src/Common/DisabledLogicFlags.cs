namespace Src.Common
{
    public static class DisabledLogicFlags
    {
        public static bool IsFriendsLogicDisabled => MirraSdkWrapper.IsYandexGames;
        public static bool IsServerDataDisabled => MirraSdkWrapper.IsYandexGames;
        public static bool IsLeaderboardsDisabled => !MirraSdkWrapper.IsVk && !MirraSdkWrapper.IsYandexGames;
    }
}