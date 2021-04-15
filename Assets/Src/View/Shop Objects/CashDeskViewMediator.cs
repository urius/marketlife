using UnityEngine;

public class CashDeskViewMediator : ShopObjectMediatorBase
{
    private readonly CashDeskModel _cashDeskModel;
    private readonly PrefabsHolder _prefabsHolder;

    public CashDeskViewMediator(Transform parentTransform, CashDeskModel cashDeskModel)
        : base(parentTransform, cashDeskModel)
    {
        _cashDeskModel = cashDeskModel;

        _prefabsHolder = PrefabsHolder.Instance;
    }

    protected override (GameObject, GameObject) GetPrefabs()
    {
        return _prefabsHolder.GetCashDeskPrefabs(_cashDeskModel.Level);
    }
}
