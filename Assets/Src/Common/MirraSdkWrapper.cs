using romanlee17.MirraGames;
using romanlee17.MirraGames.Interfaces;
using UnityEngine;

namespace Src.Common
{
    public class MirraSdkWrapper
    {
        public static readonly MirraSdkWrapper Instance = new();

        public static string PlayerId => MirraSDK.Player.PlatformToken;
        public static bool IsYandexGames => MirraSDK.Platform.Current == PlatformType.Web_YandexGames;

        public static void Log(string message)
        {
            Debug.Log($"Mirra {message}");
        }
    }
}