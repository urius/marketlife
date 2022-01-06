using UnityEngine;

public class DailyMissionRepaintFloorsProcessor : DailyMissionProcessorBase
{
    private readonly ShopModel _playerShopModel;

    public DailyMissionRepaintFloorsProcessor()
    {
        _playerShopModel = PlayerModelHolder.Instance.ShopModel;
    }

    public override void Start()
    {
        _playerShopModel.ShopDesign.FloorChanged += OnFloorChanged;
    }

    public override void Stop()
    {
        _playerShopModel.ShopDesign.FloorChanged -= OnFloorChanged;
    }

    private void OnFloorChanged(Vector2Int coords, int floorId)
    {
        MissionModel.AddValue(1);
    }
}
