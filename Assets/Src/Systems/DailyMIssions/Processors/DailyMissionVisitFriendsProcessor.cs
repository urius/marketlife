using System.Collections.Generic;
using Src.Model;

namespace Src.Systems.DailyMIssions.Processors
{
    public class DailyMissionVisitFriendsProcessor : DailyMissionProcessorBase
    {
        private readonly GameStateModel _gameStateModel;
        private readonly PlayerModelHolder _playerModelHolder;

        //
        private List<string> _visitedUids = new List<string>();

        public DailyMissionVisitFriendsProcessor()
        {
            _gameStateModel = GameStateModel.Instance;
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void Start()
        {
            _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        }

        public override void Stop()
        {
            _gameStateModel.ViewingUserModelChanged -= OnViewingUserModelChanged;
        }

        private void OnViewingUserModelChanged(UserModel userModel)
        {
            if (userModel.Uid != _playerModelHolder.Uid)
            {
                if (_visitedUids.Contains(userModel.Uid) == false)
                {
                    _visitedUids.Add(userModel.Uid);
                    MissionModel.AddValue(1);
                }
            }
        }
    }
}
