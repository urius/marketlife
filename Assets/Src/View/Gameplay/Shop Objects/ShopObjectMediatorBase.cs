using UnityEngine;

public abstract class ShopObjectMediatorBase
{
    protected readonly Transform ParentTransform;
    protected readonly ShopObjectBase Model;

    private GameObject _currentViewGo;
    private bool _hasProfileSide;
    private (GameObject, GameObject) _prefabs;

    public ShopObjectMediatorBase(Transform parentTransform, ShopObjectBase shelfModel)
    {
        ParentTransform = parentTransform;
        Model = shelfModel;
    }

    public virtual void Mediate()
    {
        _prefabs = PrefabsHolder.Instance.GetShopObjectPrefabs(Model.Type, Model.Level);
        _hasProfileSide = _prefabs.Item2 != null;
        CreateView();
        Activate();
    }

    public virtual void Unmediate()
    {
        Deactivate();
        DestroyViews();
    }

    private void CreateView()
    {
        var side = Model.Side;
        var prefab = (_hasProfileSide && SideHelper.IsProfileSide(side)) ? _prefabs.Item2 : _prefabs.Item1;
        var viewGo = GameObject.Instantiate(prefab, ParentTransform);
        _currentViewGo = viewGo;
        UpdatePosition();
        var scaleXMultiplier = SideHelper.GetScaleXMultiplier(side);
        viewGo.transform.localScale = new Vector3(scaleXMultiplier, 1, 1);
    }

    private void Activate()
    {
        Model.CoordsChanged += OnCoordsChanged;
    }

    private void Deactivate()
    {
        Model.CoordsChanged -= OnCoordsChanged;
    }

    private void OnCoordsChanged(Vector2Int oldCoords, Vector2Int newCoords)
    {
        UpdatePosition();
    }

    private void DestroyViews()
    {
        if (_currentViewGo != null)
        {
            GameObject.Destroy(_currentViewGo);
        }
    }

    private void UpdatePosition()
    {
        _currentViewGo.transform.position = GridCalculator.Instance.CellToWord(Model.Coords);
    }
}
