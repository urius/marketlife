using System;
using UnityEngine;

public class CustomerMediator : IMediator
{
    private Vector3 _startWorldCoords;
    private Vector3 _targetWorldCoords;
    private Vector3 _moveDirection;
    private bool _needToChangeSortingLayer;
    private Action _gameplayTimeUpdateDelegate;
    private int _animationCooldownFrames;

    private readonly Transform _parentTransform;
    private readonly CustomerModel _customerModel;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GridCalculator _gridCalculator;
    private readonly UpdatesProvider _updatesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly ShoDesignModel _shopDesignModel;

    private HumanView _humanView;
    private bool _isActive;

    public CustomerMediator(Transform parentTransform, CustomerModel customerModel)
    {
        _parentTransform = parentTransform;
        _customerModel = customerModel;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _gridCalculator = GridCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _shopDesignModel = GameStateModel.Instance.ViewingShopModel.ShopDesign;
    }

    public void Mediate()
    {
        var humanGo = GameObject.Instantiate(_prefabsHolder.Human, _parentTransform);
        _humanView = humanGo.GetComponent<HumanView>();
        _humanView.SetSortingLayer(SortingLayers.OrderableOutside);
        _humanView.SetupSkins(_customerModel.HairId, _customerModel.TopClothesId, _customerModel.BottomClothesId);
        _humanView.SetGlasses(_customerModel.GlassesId);

        UpdateSortingLayer();
        _humanView.transform.position = _gridCalculator.CellToWorld(_customerModel.Coords);
        DisplaySide();
        DisplayMood();

        _isActive = _gameStateModel.IsSimulationState;

        Activate();
    }

    public void Unmediate()
    {
        Deactivate();

        GameObject.Destroy(_humanView.gameObject);
    }

    private void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.PausedStateChanged += OnPauseStateChanged;
        _customerModel.CoordsChanged += OnCoordsChanged;
        _customerModel.SideChanged += OnSideChanged;
        _customerModel.AnimationStateChanged += OnAnimationStateChanged;
        _customerModel.MoodChanged += OnMoodChanged;
        _updatesProvider.GametimeUpdate += OnGameplayTimeUpdate;
    }

    private void Deactivate()
    {
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
        _gameStateModel.PausedStateChanged -= OnPauseStateChanged;
        _customerModel.CoordsChanged -= OnCoordsChanged;
        _customerModel.SideChanged -= OnSideChanged;
        _customerModel.AnimationStateChanged -= OnAnimationStateChanged;
        _customerModel.MoodChanged -= OnMoodChanged;
        _updatesProvider.GametimeUpdate -= OnGameplayTimeUpdate;
    }

    private void OnPauseStateChanged()
    {
        if (_gameStateModel.IsGamePaused)
        {
            _humanView.SetBodyState(BodyState.Idle);
        }
        else
        {
            UpdateAnimationState();
        }
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        _isActive = _gameStateModel.IsSimulationState;
        _humanView.gameObject.SetActive(_isActive);
    }

    private void OnMoodChanged()
    {
        DisplayMood();
    }

    private void DisplayMood()
    {
        _humanView.ShowFaceAnimation((int)_customerModel.Mood);
    }

    private void OnAnimationStateChanged()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        switch (_customerModel.AnimationState)
        {
            case CustomerAnimationState.Thinking:
                _animationCooldownFrames = UnityEngine.Random.Range(100, 200);
                _humanView.SetBodyState(BodyState.Idle);
                _gameplayTimeUpdateDelegate = WaitingDelegate;
                break;
            case CustomerAnimationState.Idle:
                _humanView.SetBodyState(BodyState.Idle);
                break;
            case CustomerAnimationState.Moving:
                _humanView.SetBodyState(BodyState.Walking);
                break;
            case CustomerAnimationState.TakingProduct:
                _animationCooldownFrames = 100;
                _humanView.SetBodyState(BodyState.Taking);
                _gameplayTimeUpdateDelegate = WaitingDelegate;
                break;
            case CustomerAnimationState.Paying:
                _animationCooldownFrames = 120;
                _humanView.SetBodyState(BodyState.Taking);
                _gameplayTimeUpdateDelegate = WaitingDelegate;
                break;
        }
    }

    private void OnSideChanged(int prevSide, int currentSide)
    {
        DisplaySide();
    }

    private void DisplaySide()
    {
        _humanView.ShowSide(_customerModel.Side);
    }

    private void OnGameplayTimeUpdate()
    {
        if (_isActive)
        {
            _gameplayTimeUpdateDelegate?.Invoke();
        }
    }

    private void OnCoordsChanged(PositionableObjectModelBase customerModel, Vector2Int prevCoords, Vector2Int currentCoords)
    {
        _startWorldCoords = _gridCalculator.CellToWorld(prevCoords);
        _targetWorldCoords = _gridCalculator.CellToWorld(currentCoords);
        _moveDirection = (_targetWorldCoords - _gridCalculator.CellToWorld(prevCoords)).normalized * 0.04f;
        _needToChangeSortingLayer = _shopDesignModel.IsCellInside(currentCoords) != _shopDesignModel.IsCellInside(prevCoords);

        _gameplayTimeUpdateDelegate = MovingAnimationDelegate;
    }

    private void MovingAnimationDelegate()
    {
        _humanView.transform.position += _moveDirection;
        var restSqrDistance = (_targetWorldCoords - _humanView.transform.position).sqrMagnitude;
        if (_needToChangeSortingLayer)
        {
            var passedSqrDistance = (_startWorldCoords - _humanView.transform.position).sqrMagnitude;
            if (passedSqrDistance > restSqrDistance)
            {
                _needToChangeSortingLayer = false;
                UpdateSortingLayer();
            }
        }
        if (restSqrDistance < 0.002f)
        {
            _humanView.transform.position = _targetWorldCoords;
            _gameplayTimeUpdateDelegate = null;
            _dispatcher.CustomerAnimationEnded(_customerModel);
        }
    }

    private void UpdateSortingLayer()
    {
        var sortingLayername = _shopDesignModel.IsCellInside(_customerModel.Coords) ? SortingLayers.Default : SortingLayers.OrderableOutside;
        _humanView.SetSortingLayer(sortingLayername);
    }

    private void WaitingDelegate()
    {
        _animationCooldownFrames--;
        if (_animationCooldownFrames <= 0)
        {
            _gameplayTimeUpdateDelegate = null;
            _dispatcher.CustomerAnimationEnded(_customerModel);
        }
    }
}
