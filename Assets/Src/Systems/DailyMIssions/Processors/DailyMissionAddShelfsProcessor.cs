using System.Linq;

public class DailyMissionAddShelfsProcessor : DailyMissionProcessorBase
{
    private readonly DailyMissionAddShelfsModel _missionModel;
    private readonly PlayerModelHolder _playerModelHolder;

    public DailyMissionAddShelfsProcessor(DailyMissionAddShelfsModel missionModel)
        : base(missionModel)
    {
        _missionModel = missionModel;

        _playerModelHolder = PlayerModelHolder.Instance;
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
