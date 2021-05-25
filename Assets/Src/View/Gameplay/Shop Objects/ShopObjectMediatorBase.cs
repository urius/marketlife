using UnityEngine;

public abstract class ShopObjectMediatorBase
{
    protected readonly Transform ParentTransform;
    protected readonly ShopObjectModelBase Model;

    private readonly bool _hasProfileSide;
    private readonly Dispatcher _dispatcher;
    private readonly MouseCellCoordsProvider _mouseCellCoordsProvider;
    private readonly GameStateModel _gameStateModel;
    private readonly (GameObject, GameObject) _prefabs;

    private ShopObjectViewBase _currentView;
    private ShopObjectViewBase _defaultSideView;
    private ShopObjectViewBase _profileSideView;
    private Vector2Int _hoveredMouseCellCoords;

    public ShopObjectMediatorBase(Transform parentTransform, ShopObjectModelBase model)
    {
        ParentTransform = parentTransform;
        Model = model;

        _prefabs = PrefabsHolder.Instance.GetShopObjectPrefabs(Model.Type, Model.NumericId);
        _hasProfileSide = _prefabs.Item2 != null;

        _dispatcher = Dispatcher.Instance;
        _mouseCellCoordsProvider = MouseCellCoordsProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
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
        if (_currentView != _defaultSideView && _defaultSideView != null)
        {
            GameObject.Destroy(_defaultSideView.gameObject);
            _defaultSideView = null;
        }
        if (_currentView != _profileSideView && _profileSideView != null)
        {
            GameObject.Destroy(_profileSideView.gameObject);
            _profileSideView = null;
        }
    }

    private ShopObjectViewBase GetOrCreatRotatedView(int side)
    {
        ShopObjectViewBase result;
        var isProfileSide = _hasProfileSide && SideHelper.IsProfileSide(side);
        result = isProfileSide ? _profileSideView : _defaultSideView;
        if (result == null)
        {
            var prefab = isProfileSide ? _prefabs.Item2 : _prefabs.Item1;
            var go = GameObject.Instantiate(prefab, ParentTransform);
            result = go.GetComponent<ShopObjectViewBase>();
            if (isProfileSide)
            {
                _profileSideView = result;
            }
            else
            {
                _defaultSideView = result;
            }
        }

        result.gameObject.SetActive(true);
        var scaleXMultiplier = SideHelper.GetScaleXMultiplier(side);
        result.transform.localScale = new Vector3(scaleXMultiplier, 1, 1);

        return result;
    }

    private void Activate()
    {
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        Model.CoordsChanged += OnCoordsChanged;
        Model.SideChanged += OnSideChanged;
        Model.Hovered += OnHovered;
    }

    private void Deactivate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        Model.CoordsChanged -= OnCoordsChanged;
        Model.SideChanged -= OnSideChanged;
        Model.Hovered -= OnHovered;
    }

    private void OnHovered()
    {
        if (_gameStateModel.PlacingState == PlacingStateName.None)
        {
            SetIsHovered(true);
        }
    }

    private void OnMouseCellCoordsUpdated(Vector2Int newCoords)
    {
        if (_hoveredMouseCellCoords != newCoords)
        {
            SetIsHovered(false);
        }
    }

    private void SetIsHovered(bool isHovered)
    {
        if (isHovered)
        {
            _hoveredMouseCellCoords = _mouseCellCoordsProvider.MouseCellCoords;
        }
        _currentView.SetAllSpritesAlpha(isHovered ? 0.5f : 1);
    }

    private void OnSideChanged(int previousSide, int newSide)
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (_currentView != null)
        {
            _currentView.gameObject.SetActive(false);
        }
        _currentView = GetOrCreatRotatedView(Model.Side);
        UpdatePosition();
    }

    private void OnCoordsChanged(Vector2Int oldCoords, Vector2Int newCoords)
    {
        UpdatePosition();
    }

    private void DestroyViews()
    {
        DestroyNotDisplayedSideView();
        if (_currentView != null)
        {
            GameObject.Destroy(_currentView.gameObject);
            _currentView = null;
        }
    }

    private void UpdatePosition()
    {
        _currentView.transform.position = GridCalculator.Instance.CellToWord(Model.Coords);
    }
}
