using System.Linq;
using Src.Common;
using UnityEngine;

public class BillboardMediator : IMediator
{
    private readonly Transform _contentTransform;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GridCalculator _gridCalculator;
    private readonly UpdatesProvider _updatesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
    private readonly Vector2Int _billboardCellCoords;
    private readonly HighlightableMediatorComponent _highlightableComponent;

    //
    private BillboardView _billboardView;
    private ShopModel _currentShopModel;
    private int _updateFramesCount;
    private bool _isHiglighted;

    public BillboardMediator(Transform contentTransform, Vector2Int coords)
    {
        _contentTransform = contentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _gridCalculator = GridCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;

        _billboardCellCoords = coords;
        _highlightableComponent = new HighlightableMediatorComponent();
    }

    public void Mediate()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            HandleNewShopModel(_gameStateModel.ViewingShopModel);
        }
    }

    public void Unmediate()
    {
        Deactivate();

        ForgetCurrentShopModel();
        if (_billboardView != null)
        {
            GameObject.Destroy(_billboardView.gameObject);
            _billboardView = null;
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        _dispatcher.UIGameViewMouseClick += OnGameViewMouseClick;
    }

    private void Deactivate()
    {
        _gameStateModel.ViewingUserModelChanged -= OnViewingUserModelChanged;
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        _dispatcher.UIGameViewMouseClick -= OnGameViewMouseClick;
    }

    private void OnRealtimeUpdate()
    {
        _updateFramesCount++;
        if (_updateFramesCount >= 3)
        {
            _updateFramesCount = 0;
            ProcessHighlight();
        }
    }

    private void ProcessHighlight()
    {
        if (_gameStateModel.GameState == GameStateName.PlayerShopSimulation
            && _gameStateModel.ActionState == ActionStateName.None)
        {
            if (_billboardView != null && _billboardView.gameObject.activeSelf == true)
            {
                var isHighlighted = _highlightableComponent.CheckMousePoint();
                HighlightView(isHighlighted);
            }
        }
        else
        {
            HighlightView(false);
        }
    }

    private void HighlightView(bool isHighlighted)
    {
        if (_isHiglighted != isHighlighted)
        {
            _isHiglighted = isHighlighted;
            _billboardView.SetIsHiglighted(_isHiglighted);
        }
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        HandleNewShopModel(userModel.ShopModel);
    }

    private void HandleNewShopModel(ShopModel viewingShopModel)
    {
        ForgetCurrentShopModel();

        _currentShopModel = viewingShopModel;

        _currentShopModel.BillboardModel.AvailabilityChanged += OnBillboardAvailabilityChanged;
        _currentShopModel.BillboardModel.TextChanged += OnBillboardTextChanged;

        DisplayBillboard();
    }

    private void ForgetCurrentShopModel()
    {
        if (_currentShopModel != null)
        {
            _currentShopModel.BillboardModel.AvailabilityChanged -= OnBillboardAvailabilityChanged;
            _currentShopModel.BillboardModel.TextChanged -= OnBillboardTextChanged;
            _currentShopModel = null;
        }
    }

    private void OnBillboardTextChanged()
    {
        DisplayBillboard();
    }

    private void OnBillboardAvailabilityChanged()
    {
        DisplayBillboard();
    }

    private void DisplayBillboard()
    {
        if (_currentShopModel.BillboardModel.IsAvailable == true)
        {
            if (_billboardView == null)
            {
                var go = GameObject.Instantiate(_prefabsHolder.BillboardPrefab, _contentTransform);
                go.transform.position = _gridCalculator.CellToWorld(_billboardCellCoords);
                _billboardView = go.GetComponent<BillboardView>();
                var boundPoints = _billboardView.BoundPointTransforms.Select(t => t.position).ToArray();
                _highlightableComponent.UpdateBoundPoints(boundPoints);
                _tutorialUIElementsProvider.SetElement(TutorialUIElement.BillboardTransform, _billboardView.transform);
            }

            _billboardView.gameObject.SetActive(true);
            _billboardView.SetText(_currentShopModel.BillboardModel.Text);
        }
        else
        {
            if (_billboardView != null)
            {
                _billboardView.gameObject.SetActive(false);
            }
        }
    }

    private void OnGameViewMouseClick()
    {
        if (_highlightableComponent.CheckMousePoint())
        {
            _dispatcher.UIDispatchBillboardClick();
        }
    }
}
