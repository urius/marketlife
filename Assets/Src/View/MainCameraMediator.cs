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
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
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
    }

    private void ProcessMouseCellPositionCalculation()
    {
        if (_previousMousePosition != _currentMousePosition)
        {
            _framesCounter3++;
            if (_framesCounter3 >= 3)
            {
                _framesCounter3 = 0;
                var mouseWorld = GetOnPlaneMouseWorldPoint();
                var mouseCell = _gridCalculator.WorldToCell(mouseWorld);
                _mouseCellCoordsProvider.SetMouseCellCoords(mouseCell);
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
    }

    private Vector3 GetOnPlaneMouseWorldPoint()
    {
        var cameraTransform = _camera.transform;
        var mousePosition = _currentMousePosition;
        var mouseWorldPoint = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y));
        var rotationX = Mathf.Deg2Rad * cameraTransform.rotation.eulerAngles.x;
        var distance = (float)(Math.Abs(mouseWorldPoint.z) / Math.Cos(rotationX));

        var result = mouseWorldPoint + cameraTransform.forward * distance;

        return result;
    }
}
