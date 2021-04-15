using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly ShelfModel _shelfModel;
    private readonly PrefabsHolder _prefabsHolder;

    public ShelfViewMediator(Transform parentTransform, ShelfModel shelfModel)
        : base(parentTransform, shelfModel)
    {
        _shelfModel = shelfModel;

        _prefabsHolder = PrefabsHolder.Instance;
    }

    protected override (GameObject, GameObject) GetPrefabs()
    {
        return _prefabsHolder.GetShelfPrefabs(_shelfModel.Level);
    }
}
