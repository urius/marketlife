using UnityEngine;

public abstract class ShopObjectMediatorBase
{
    protected readonly Transform ParentTransform;
    protected readonly ShopObjectBase Model;

    private readonly bool _hasProfileSide;
    private readonly (GameObject, GameObject) _prefabs;

    private GameObject _currentViewGo;
    private GameObject _defaultSideViewGo;
    private GameObject _profileSideViewGo;

    public ShopObjectMediatorBase(Transform parentTransform, ShopObjectBase shelfModel)
    {
        ParentTransform = parentTransform;
        Model = shelfModel;

        _prefabs = PrefabsHolder.Instance.GetShopObjectPrefabs(Model.Type, Model.Level);
        _hasProfileSide = _prefabs.Item2 != null;
    }

    public virtual void Mediate()
    {
        UpdateView();
        Activate();
    }

    public virtual void Unmediate()
    {
        Deactivate();
        DestroyViews();
    }

    protected void DestroyNotDisplayedSideView()
    {
        if (_currentViewGo != _defaultSideViewGo && _defaultSideViewGo != null)
        {
            GameObject.Destroy(_defaultSideViewGo);
            _defaultSideViewGo = null;
        }
        if (_currentViewGo != _profileSideViewGo && _profileSideViewGo != null)
        {
            GameObject.Destroy(_profileSideViewGo);
            _profileSideViewGo = null;
        }
    }

    private GameObject GetOrCreatRotatedView(int side)
    {
        GameObject result;
        var isProfileSide = _hasProfileSide && SideHelper.IsProfileSide(side);
        result = isProfileSide ? _profileSideViewGo : _defaultSideViewGo;
        if (result == null)
        {
            var prefab = isProfileSide ? _prefabs.Item2 : _prefabs.Item1;
            result = GameObject.Instantiate(prefab, ParentTransform);
            if (isProfileSide)
            {
                _profileSideViewGo = result;
            }
            else
            {
                _defaultSideViewGo = result;
            }
        }

        result.SetActive(true);
        var scaleXMultiplier = SideHelper.GetScaleXMultiplier(side);
        result.transform.localScale = new Vector3(scaleXMultiplier, 1, 1);

        return result;
    }

    private void Activate()
    {
        Model.CoordsChanged += OnCoordsChanged;
        Model.SideChanged += OnSideChanged;
    }

    private void Deactivate()
    {
        Model.CoordsChanged -= OnCoordsChanged;
        Model.SideChanged -= OnSideChanged;
    }

    private void OnSideChanged(int previousSide, int newSide)
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (_currentViewGo != null)
        {
            _currentViewGo.SetActive(false);
        }
        _currentViewGo = GetOrCreatRotatedView(Model.Side);
        UpdatePosition();
    }

    private void OnCoordsChanged(Vector2Int oldCoords, Vector2Int newCoords)
    {
        UpdatePosition();
    }

    private void DestroyViews()
    {
        DestroyNotDisplayedSideView();
        if (_currentViewGo != null)
        {
            GameObject.Destroy(_currentViewGo);
            _currentViewGo = null;
        }
    }

    private void UpdatePosition()
    {
        _currentViewGo.transform.position = GridCalculator.Instance.CellToWord(Model.Coords);
    }
}
