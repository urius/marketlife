using Src.Model;

namespace Src.Systems.DailyMIssions.Processors
{
    public class DailyMissionUpgradeWarehouseVolumeProcessor : DailyMissionProcessorBase
    {
        private readonly PlayerModelHolder _playerModelHolder;

        public DailyMissionUpgradeWarehouseVolumeProcessor()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void Start()
        {
            _playerModelHolder.ShopModel.WarehouseModel.VolumeAdded += OnVolumeAdded;
        }

        public override void Stop()
        {
            _playerModelHolder.ShopModel.WarehouseModel.VolumeAdded -= OnVolumeAdded;
        }

        private void OnVolumeAdded(int delta)
        {
            var newVolume = _playerModelHolder.ShopModel.WarehouseModel.Volume;
            MissionModel.SetValue(newVolume);
        }
    }
}
