using UnityEngine;

public abstract class ShopObjectMediatorBase
{
    protected Transform ParentTransform;

    private readonly ShopObjectBase _model;

    private GameObject _currentViewGo;
    private bool _hasProfileSide;
    private (GameObject, GameObject) _prefabs;

    public ShopObjectMediatorBase(Transform parentTransform, ShopObjectBase shelfModel)
    {
        ParentTransform = parentTransform;
        _model = shelfModel;
    }

    public void Mediate()
    {
        _prefabs = PrefabsHolder.Instance.GetShopObjectPrefabs(_model.Type, _model.Level);
        _hasProfileSide = _prefabs.Item2 != null;
        CreateView();
    }

    public virtual void Destroy()
    {
        if (_currentViewGo != null)
        {
            GameObject.Destroy(_currentViewGo);
        }
    }

    protected virtual void CreateView()
    {
        var side = _model.Side;
        var prefab = (_hasProfileSide && SideHelper.IsProfileSide(side)) ? _prefabs.Item2 : _prefabs.Item1;
        var viewGo = GameObject.Instantiate(prefab, ParentTransform);
        _currentViewGo = viewGo;
        viewGo.transform.position = GridCalculator.Instance.CellToWord(_model.Coords);
        var scaleXMultiplier = SideHelper.GetScaleXMultiplier(side);
        viewGo.transform.localScale = new Vector3(scaleXMultiplier, 1, 1);
    }
}
