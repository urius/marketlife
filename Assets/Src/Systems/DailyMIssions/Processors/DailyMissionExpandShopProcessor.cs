using Src.Model;

namespace Src.Systems.DailyMIssions.Processors
{
    public class DailyMissionExpandShopProcessor : DailyMissionProcessorBase
    {
        private readonly PlayerModelHolder _playerModelHolder;

        public DailyMissionExpandShopProcessor()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void Start()
        {
            _playerModelHolder.ShopModel.ShopDesign.SizeXChanged += OnSizeXChanged;
            _playerModelHolder.ShopModel.ShopDesign.SizeYChanged += OnSizeYChanged;
        }

        public override void Stop()
        {
            _playerModelHolder.ShopModel.ShopDesign.SizeXChanged -= OnSizeXChanged;
            _playerModelHolder.ShopModel.ShopDesign.SizeYChanged -= OnSizeYChanged;
        }

        private void OnSizeXChanged(int prevSize, int currentSize)
        {
            MissionModel.AddValue(currentSize - prevSize);
        }

        private void OnSizeYChanged(int prevSize, int currentSize)
        {
            MissionModel.AddValue(currentSize - prevSize);
        }
    }
}
