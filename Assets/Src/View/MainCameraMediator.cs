using System;
using UnityEngine;

public class MainCameraMediator : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Dispatcher _dispatcher;
    private UpdatesProvider _updatesProvider;
    private GridCalculator _gridCalculator;
    private MouseCellCoordsProvider _mouseCellCoordsProvider;
    private bool _isMouseDown;
    private Vector3 _mouseDownWorldPosition;
    private Vector3 _deltaWorldMouse;
    private float _cameraZ;
    private int _framesCounter3;
    private Vector3 _previousMousePosition;
    private Vector3 _currentMousePosition;
    private bool _needToUpdateMouseCellPosition;
    private bool _needToForceUpdateMouseCellPosition;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
        _mouseCellCoordsProvider = MouseCellCoordsProvider.Instance;
    }

    private void Start()
    {
        _cameraZ = transform.position.z;

        _dispatcher.UIGameViewMouseDown += OnGameViewMouseDown;
        _dispatcher.UIGameViewMouseUp += OnGameViewMouseUp;
        _dispatcher.UIGameViewMouseEnter += OnGameViewMouseEnter;
        _dispatcher.UIGameViewMouseExit += OnGameViewMouseExit;
        _dispatcher.RequestForceMouseCellPositionUpdate += OnRequestForceMouseCellPositionUpdate;

        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
    }

    private void OnRequestForceMouseCellPositionUpdate()
    {
        _needToForceUpdateMouseCellPosition = true;
    }

    private void OnGameViewMouseEnter()
    {
        _needToUpdateMouseCellPosition = true;
    }

    private void OnGameViewMouseExit()
    {
        _needToUpdateMouseCellPosition = false;
    }

    private void OnGameViewMouseDown()
    {
        _mouseDownWorldPosition = GetOnPlaneMouseWorldPoint();
        _isMouseDown = true;
    }

    private void OnGameViewMouseUp()
    {
        _isMouseDown = false;
    }

    private void OnRealtimeUpdate()
    {
        _previousMousePosition = _currentMousePosition;
        _currentMousePosition = Input.mousePosition;
        ProcessCameraMove();
        ProcessMouseCellPositionCalculation();
        if (_currentMousePosition != _previousMousePosition)
        {
            _dispatcher.MouseMoved();
        }
    }

    private void ProcessMouseCellPositionCalculation()
    {
        if (_previousMousePosition != _currentMousePosition || _needToForceUpdateMouseCellPosition)
        {
            _framesCounter3++;
            if (_framesCounter3 >= 3 && (_needToUpdateMouseCellPosition || _needToForceUpdateMouseCellPosition))
            {
                _framesCounter3 = 0;
                var mouseWorld = GetOnPlaneMouseWorldPoint();
                var mouseCell = _gridCalculator.WorldToCell(mouseWorld);
                _mouseCellCoordsProvider.SetMouseCellCoords(mouseCell, _needToForceUpdateMouseCellPosition);
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
        newCameraPos.z = _cameraZ;
        _camera.transform.position = newCameraPos;
        _dispatcher.CameraMoved(_deltaWorldMouse);
    }

    private Vector3 GetOnPlaneMouseWorldPoint()
    {
        return _gridCalculator.ScreenPointToPlaneWorldPoint(_camera, _currentMousePosition);
    }
}
