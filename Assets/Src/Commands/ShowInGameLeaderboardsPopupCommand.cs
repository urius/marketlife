using Src.Model.Popups;

namespace Src.Commands
{
    public struct ShowInGameLeaderboardsPopupCommand
    {
        public async void Execute()
        {
            var dispatcher = Dispatcher.Instance;
            dispatcher.UIRequestBlockRaycasts();
            var loadLBResult = await new LoadLeaderboardsDataCommand().ExecuteAsync();
            dispatcher.UIRequestUnblockRaycasts();

            if (loadLBResult)
            {
                var gameStateModel = GameStateModel.Instance;
                var leaderboardDataHolder = LeaderboardsDataHolder.Instance;
                var popupModel = new LeaderboardsPopupViewModel(
                    leaderboardDataHolder.GetLeaderboardData(LeaderboardType.Exp),
                    leaderboardDataHolder.GetLeaderboardData(LeaderboardType.Cash),
                    leaderboardDataHolder.GetLeaderboardData(LeaderboardType.Gold),
                    leaderboardDataHolder.GetLeaderboardData(LeaderboardType.Friends));
                gameStateModel.ShowPopup(popupModel);
            }
        }
    }
}
