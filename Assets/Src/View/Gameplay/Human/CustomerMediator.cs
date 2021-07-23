using UnityEngine;

public class CustomerMediator : IMediator
{
    private readonly Transform _parentTransform;
    private readonly CustomerModel _customerModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GridCalculator _gridCalculator;

    private HumanView _humanView;

    public CustomerMediator(Transform parentTransform, CustomerModel customerModel)
    {
        _parentTransform = parentTransform;
        _customerModel = customerModel;

        _prefabsHolder = PrefabsHolder.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    public void Mediate()
    {
        var humanGo = GameObject.Instantiate(_prefabsHolder.Human, _parentTransform);
        _humanView = humanGo.GetComponent<HumanView>();

        _humanView.transform.position = _gridCalculator.CellToWord(_customerModel.Coords);
    }

    public void Unmediate()
    {
        GameObject.Destroy(_humanView.gameObject);
    }
}
