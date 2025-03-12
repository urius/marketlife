namespace Src.Common
{
    public static class DisabledLogicFlags
    {
        public static bool IsFriendsLogicDisabled = false;
        public static bool IsServerDataDisabled => MirraSdkWrapper.IsYandexGames;
    }
}