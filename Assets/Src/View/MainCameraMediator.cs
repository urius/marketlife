using System;
using UnityEngine;

public class MainCameraMediator : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Dispatcher _dispatcher;

    private bool _isDragging;
    private Vector3 _startDragWorldMousePos;
    private Vector3 _deltaWorldMouse;
    private float _cameraZ;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
    }

    private void Start()
    {
        _cameraZ = transform.position.z;

        _dispatcher.GameViewMouseDown += OnGameViewMouseDown;
        _dispatcher.GameViewMouseUp += OnGameViewMouseUp;
    }

    private void OnGameViewMouseDown()
    {
        _startDragWorldMousePos = GetOnPlaneMouseWorldPoint();
        _isDragging = true;
    }

    private void OnGameViewMouseUp()
    {
        _isDragging = false;
    }

    private void FixedUpdate()
    {
        ProcessCameraMove();
    }

    private void ProcessCameraMove()
    {
        if (_isDragging)
        {
            var currentMouseWorldPoint = GetOnPlaneMouseWorldPoint();
            _deltaWorldMouse = currentMouseWorldPoint - _startDragWorldMousePos;
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
        var mousePosition = Input.mousePosition;
        var mouseWorldPoint = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y));
        var rotationX = Mathf.Deg2Rad * cameraTransform.rotation.eulerAngles.x;
        var distance = (float)(Math.Abs(mouseWorldPoint.z) / Math.Cos(rotationX));

        var result = mouseWorldPoint + cameraTransform.forward * distance;

        return result;
    }
}
