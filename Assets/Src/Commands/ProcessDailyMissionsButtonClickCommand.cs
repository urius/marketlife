using Src.Model;
using Src.Model.Popups;

namespace Src.Commands
{
    public struct ProcessDailyMissionsButtonClickCommand
    {
        public void Execute()
        {

            var gameStateModel = GameStateModel.Instance;
            var playerModelHolder = PlayerModelHolder.Instance;
            var playerMissionsModel = playerModelHolder.UserModel.DailyMissionsModel;

            var popupModel = new DailyMissionsPopupViewModel(playerMissionsModel);
            gameStateModel.ShowPopup(popupModel);
        }
    }
}
