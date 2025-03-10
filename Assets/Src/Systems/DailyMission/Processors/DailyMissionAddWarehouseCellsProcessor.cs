using Src.Model;

namespace Src.Systems.DailyMission.Processors
{
    public class DailyMissionAddWarehouseCellsProcessor : DailyMissionProcessorBase
    {
        private readonly PlayerModelHolder _playerModelHolder;

        public DailyMissionAddWarehouseCellsProcessor()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void Start()
        {
            _playerModelHolder.ShopModel.WarehouseModel.SlotsAdded += OnSlotsAdded;
        }

        public override void Stop()
        {
            _playerModelHolder.ShopModel.WarehouseModel.SlotsAdded -= OnSlotsAdded;
        }

        private void OnSlotsAdded(int delta)
        {
            var newSlotsCount = _playerModelHolder.ShopModel.WarehouseModel.Size;
            MissionModel.SetValue(newSlotsCount);
        }
    }
}
