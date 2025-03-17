using Src.Common;
using UnityEngine;

namespace Src.Model.Popups
{
    public struct HandlePauseOffCommand
    {
        public void Execute()
        {
            Dispatcher.Instance.UIRequestRemoveCurrentPopup();
            Time.timeScale = 1;
        }
    }
}