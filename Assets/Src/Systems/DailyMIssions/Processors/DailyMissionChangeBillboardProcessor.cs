using Src.Model;

namespace Src.Systems.DailyMIssions.Processors
{
    public class DailyMissionChangeBillboardProcessor : DailyMissionProcessorBase
    {
        private readonly PlayerModelHolder _playerModelHolder;
        private readonly GameStateModel _gameStateModel;

        public DailyMissionChangeBillboardProcessor()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
            _gameStateModel = GameStateModel.Instance;
        }

        public override void Start()
        {
            _playerModelHolder.ShopModel.BillboardModel.TextChanged += OnBillboardTextChanged;
        }

        public override void Stop()
        {
            _playerModelHolder.ShopModel.BillboardModel.TextChanged -= OnBillboardTextChanged;
        }

        private void OnBillboardTextChanged()
        {
            if (_gameStateModel.IsPlayingState)
            {
                MissionModel.AddValue(1);
            }
        }
    }
}
