using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILoadScreenCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameLoadPanelView _loadPanelView;
    [SerializeField] private RectTransform _bgRectTransform;

    private LoadGameProgressModel _loadProgressModel;
    private LocalizationManager _loc;
    private GameStateModel _gameStateModel;
    private float _targetBarScale;
    private float _currentBarScale;

    private void Awake()
    {
        _loadProgressModel = LoadGameProgressModel.Instance;
        _loc = LocalizationManager.Instance;
        _gameStateModel = GameStateModel.Instance;

        SetBarScale(0f);
    }

    private void Start()
    {
        Activate();
    }

    private void Update()
    {
        if (_currentBarScale < _targetBarScale && _loadProgressModel.IsError == false)
        {
            //SetBarScale(_targetBarScale);
            SetBarScale(_currentBarScale + (_targetBarScale - _currentBarScale) * 0.2f);
        }
    }

    private void OnDestroy()
    {
        Deactivate();
    }

    private void Activate()
    {
        _loadProgressModel.ProgressChanged += OnLoadProgressChanged;
        _loadProgressModel.PhaseChanged += OnLoadPhaseChanged;
        _loadProgressModel.ErrorHappened += OnError;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void Deactivate()
    {
        _loadProgressModel.ProgressChanged -= OnLoadProgressChanged;
        _loadProgressModel.PhaseChanged -= OnLoadPhaseChanged;
        _loadProgressModel.ErrorHappened -= OnError;
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
    }

    private async void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        switch (currentState)
        {
            case GameStateName.Loaded:
                AnimateRemoveLoadingPanel();
                break;
            case GameStateName.ReadyForStart:
                await AnimateRemoveBgAsync();
                Destroy(gameObject);
                break;
        }
    }

    private UniTask AnimateRemoveBgAsync()
    {
        var moveData = LeanTweenHelper.MoveYAsync(_bgRectTransform.transform as RectTransform, 2 * Screen.height, 1.5f);
        moveData.tweenDescription.setEaseInOutQuad();
        return moveData.task;
    }

    private async void AnimateRemoveLoadingPanel()
    {
        var moveData = LeanTweenHelper.MoveXAsync(_loadPanelView.transform as RectTransform, -Screen.width, 0.5f);
        moveData.tweenDescription.setEaseInBack();
        await moveData.task;
    }

    private void OnLoadPhaseChanged(LoadGamePhase phase)
    {
        string key = phase switch
        {
            LoadGamePhase.LoadTime => "load_time",
            LoadGamePhase.LoadLocalization => "load_localization",
            LoadGamePhase.LoadConfigs => "load_configs",
            LoadGamePhase.LoadAssets => "load_assets",
            LoadGamePhase.LoadShopData => "load_data",
            LoadGamePhase.LoadCompensationData => "load_additional_data",
            _ => "loading",
        };
        _loadPanelView.SetStatusText(_loc.GetBuiltinLocalization(key));
    }

    private void OnError()
    {
        _loadPanelView.ShowError(
            string.Format(_loc.GetBuiltinLocalization("error_format"), _loadProgressModel.PhaseName.ToString()));
    }

    private void OnLoadProgressChanged(float progress)
    {
        _targetBarScale = progress;
    }

    private void SetBarScale(float value)
    {
        value = value > 1 ? 1 : value;
        _currentBarScale = value;
        _loadPanelView.SetBarScale(_currentBarScale);
    }
}
