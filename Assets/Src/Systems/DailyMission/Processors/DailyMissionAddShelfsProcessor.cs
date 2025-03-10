using System.Linq;
using Src.Model;
using Src.Model.Missions;
using Src.Model.ShopObjects;

namespace Src.Systems.DailyMission.Processors
{
    public class DailyMissionAddShelfsProcessor : DailyMissionProcessorBase
    {
        private readonly PlayerModelHolder _playerModelHolder;

        private DailyMissionAddShelfsModel _missionModel;

        public DailyMissionAddShelfsProcessor()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void SetupMissionModel(DailyMissionModel missionModel)
        {
            base.SetupMissionModel(missionModel);
            _missionModel = missionModel as DailyMissionAddShelfsModel;
        }

        public override void Start()
        {
            _playerModelHolder.ShopModel.ShopObjectPlaced += OnShopObjectPlaced;
            _playerModelHolder.ShopModel.ShopObjectRemoved += OnShopObjectRemoved;
        }

        public override void Stop()
        {
            _playerModelHolder.ShopModel.ShopObjectPlaced -= OnShopObjectPlaced;
            _playerModelHolder.ShopModel.ShopObjectRemoved -= OnShopObjectRemoved;
        }

        private void OnShopObjectPlaced(ShopObjectModelBase shopObject)
        {
            ProcessShopObjectsStateChange(shopObject);
        }

        private void OnShopObjectRemoved(ShopObjectModelBase shopObject)
        {
            ProcessShopObjectsStateChange(shopObject);
        }

        private void ProcessShopObjectsStateChange(ShopObjectModelBase shopObject)
        {
            var targetShelfNumericId = _missionModel.ShelfNumericId;
            if (shopObject.Type == ShopObjectType.Shelf
                && shopObject.NumericId == targetShelfNumericId)
            {
                var shelfsCount = _playerModelHolder.ShopModel.ShopObjects
                    .Count(o => o.Value.Type == ShopObjectType.Shelf && o.Value.NumericId == targetShelfNumericId);
                _missionModel.SetValue(shelfsCount);
            }
        }
    }
}
