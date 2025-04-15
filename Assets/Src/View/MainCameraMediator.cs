using System;
using Src.Common;
using UnityEngine;

public class MainCameraMediator : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private const int CameraMinSize = 3;
    private const int CameraMaxSize = 8;

    private Dispatcher _dispatcher;
    private UpdatesProvider _updatesProvider;
    private GridCalculator _gridCalculator;
    private ScreenCalculator _screenCalculator;
    private MouseDataProvider _mouseDataProvider;
    private GameStateModel _gameStateModel;
    private bool _isMouseDown;
    private Vector3 _mouseDownWorldPosition;
    private Vector3 _deltaWorldMouse;
    private float _cameraZ;
    private int _framesCounter3;
    private Vector3 _previousMousePosition;
    private Vector3 _currentMousePosition;
    private bool _isMouseOverGameView;
    private bool _needToForceUpdateMouseCellPosition;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _mouseDataProvider = MouseDataProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    private void Start()
    {
        _cameraZ = transform.position.z;

        _dispatcher.UIGameViewMouseDown += OnGameViewMouseDown;
        _dispatcher.UIGameViewMouseUp += OnGameViewMouseUp;
        _dispatcher.UIGameViewMouseEnter += OnGameViewMouseEnter;
        _dispatcher.UIGameViewMouseExit += OnGameViewMouseExit;
        _dispatcher.RequestForceMouseCellPositionUpdate += OnRequestForceMouseCellPositionUpdate;
        _dispatcher.UIRequestMoveCamera += OnUIRequestMoveCamera;
        _dispatcher.UIScaleInClicked += OnUIScaleInClicked;
        _dispatcher.UIScaleOutClicked += OnUIScaleOutClicked;
        _gameStateModel.GameStateChanged += OnGameStateChanged;

        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (_gameStateModel.CheckIsPlayingState(prevState) == false
            && _gameStateModel.IsPlayingState)
        {
            //TweenCameraSize(5, 1f);
        }
    }

    private void OnUIScaleInClicked()
    {
        var newSize = (int)_camera.orthographicSize - 1;
        if (newSize >= CameraMinSize)
        {
            TweenCameraSize(newSize, .2f);
        }
        else
        {
            _camera.orthographicSize = CameraMinSize;
        }
    }

    private void OnUIScaleOutClicked()
    {
        var newSize = (int)_camera.orthographicSize + 1;
        if (newSize <= CameraMaxSize)
        {
            TweenCameraSize(newSize, .2f);
        }
        else
        {
            _camera.orthographicSize = CameraMaxSize;
        }
    }

    private void TweenCameraSize(int targetSize, float duration)
    {
        LeanTween.cancel(_camera.gameObject);
        LeanTween.value(_camera.gameObject, f => _camera.orthographicSize = f, _camera.orthographicSize, targetSize, duration);
    }

    private void OnRequestForceMouseCellPositionUpdate()
    {
        _needToForceUpdateMouseCellPosition = true;
    }

    private void OnGameViewMouseEnter()
    {
        _isMouseOverGameView = true;
        _mouseDataProvider.SetMouseOverGameView(_isMouseOverGameView);
    }

    private void OnGameViewMouseExit()
    {
        _isMouseOverGameView = false;
        _mouseDataProvider.SetMouseOverGameView(_isMouseOverGameView);
    }

    private void OnGameViewMouseDown()
    {
        _mouseDownWorldPosition = GetOnPlaneMouseWorldPoint();
        _isMouseDown = true;
        UpdateMousePositionValues();

        _needToForceUpdateMouseCellPosition |= _isMouseOverGameView;
        ProcessMouseCellPositionCalculation();
    }

    private void OnGameViewMouseUp()
    {
        _isMouseDown = false;
    }

    private void OnRealtimeUpdate()
    {
        UpdateMousePositionValues();
        ProcessCameraMove();
        ProcessMouseCellPositionCalculation();
        if (_currentMousePosition != _previousMousePosition)
        {
            _dispatcher.UIMouseMoved();
            if (_isMouseOverGameView == true)
            {
                _dispatcher.UIGameViewMouseMoved();
            }
        }
    }

    private void UpdateMousePositionValues()
    {
        _previousMousePosition = _currentMousePosition;
        _currentMousePosition = Input.mousePosition;
    }

    private void ProcessMouseCellPositionCalculation()
    {
        if (_previousMousePosition != _currentMousePosition || _needToForceUpdateMouseCellPosition)
        {
            _framesCounter3++;
            if (_framesCounter3 >= 3 && (_isMouseOverGameView || _needToForceUpdateMouseCellPosition))
            {
                _framesCounter3 = 0;
                var mouseWorld = GetOnPlaneMouseWorldPoint();
                var mouseCell = _gridCalculator.WorldToCell(mouseWorld);
                _mouseDataProvider.SetMouseProjectedOnPlaneWorldPosition(_screenCalculator.ScreenPointToPlaneWorldPoint(_currentMousePosition));
                _mouseDataProvider.SetMouseCellCoords(mouseCell, _needToForceUpdateMouseCellPosition);
                _needToForceUpdateMouseCellPosition = false;
            }
        }
    }

    private void ProcessCameraMove()
    {
        if (_isMouseDown)
        {
            var currentMouseWorldPoint = GetOnPlaneMouseWorldPoint();
            _deltaWorldMouse = currentMouseWorldPoint - _mouseDownWorldPosition;
        }
        else if (_deltaWorldMouse.magnitude > 0.05f)
        {
            _deltaWorldMouse *= 0.9f;
        }
        else
        {
            _deltaWorldMouse = Vector3.zero;
            return;
        }

        var newCameraPos = _camera.transform.position - _deltaWorldMouse;
        _camera.transform.position = CorrectCameraCoords(newCameraPos);
        _dispatcher.CameraMoved(_deltaWorldMouse);
    }

    private Vector3 CorrectCameraCoords(Vector3 newCameraPos)
    {
        newCameraPos.z = _cameraZ;
        return newCameraPos;
    }

    private void OnUIRequestMoveCamera(Vector3 targetWorldPlanePos, float durationSeconds)
    {
        MoveCameraViewToPlanePointAnimated(targetWorldPlanePos, durationSeconds);
    }

    private void MoveCameraViewToPlanePointAnimated(Vector3 targetWorldPlanePos, float durationSeconds)
    {
        LeanTween.cancel(_camera.gameObject);

        var cameraTargetPos = CorrectCameraCoords(_screenCalculator.PlaneToCameraProjection(targetWorldPlanePos));
        LeanTween.move(_camera.gameObject, cameraTargetPos, durationSeconds).setEaseOutCubic();
    }

    private Vector3 GetOnPlaneMouseWorldPoint()
    {
        return _screenCalculator.ScreenPointToPlaneWorldPoint(_currentMousePosition);
    }
}
