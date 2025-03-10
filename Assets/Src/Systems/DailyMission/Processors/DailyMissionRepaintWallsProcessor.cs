using Src.Model;
using UnityEngine;

namespace Src.Systems.DailyMission.Processors
{
    public class DailyMissionRepaintWallsProcessor : DailyMissionProcessorBase
    {
        private readonly ShopModel _playerShopModel;

        public DailyMissionRepaintWallsProcessor()
        {
            _playerShopModel = PlayerModelHolder.Instance.ShopModel;
        }

        public override void Start()
        {
            _playerShopModel.ShopDesign.WallChanged += OnWallChanged;
        }

        public override void Stop()
        {
            _playerShopModel.ShopDesign.WallChanged -= OnWallChanged;
        }

        private void OnWallChanged(Vector2Int coords, int wallId)
        {
            MissionModel.AddValue(1);
        }
    }
}
