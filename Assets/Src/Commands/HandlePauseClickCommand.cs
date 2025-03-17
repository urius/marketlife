using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.Commands
{
    public struct HandlePauseClickCommand
    {
        public void Execute()
        {
            var gameStateModel = GameStateModel.Instance;
            
            if (gameStateModel.ShowingPopupModel?.PopupType != PopupType.Pause)
            {
                Time.timeScale = 0;
                gameStateModel.ShowPopup(new PausePopupViewModel());
            }
            else
            {
                new HandlePauseOffCommand().Execute();
            }
        }
    }
}