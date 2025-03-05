using UnityEngine.Events;
using UnityEngine.UI;

namespace Src.Common_Utils
{
    public static class UnityButtonExtensions
    {
        public static void AddOnClickListener(this Button button, UnityAction handler)
        {
            button.onClick.AddListener(handler);
        }

        public static void RemoveOnClickListener(this Button button, UnityAction handler)
        {
            button.onClick.RemoveListener(handler);
        }

        public static void RemoveAllOnClickListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
