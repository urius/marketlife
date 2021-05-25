using System;
using UnityEngine;

public class ScreenCalculator
{
    private static readonly Lazy<ScreenCalculator> _instance = new Lazy<ScreenCalculator>();
    public static ScreenCalculator Instance => _instance.Value;

    private Camera _camera;

    public Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        return GetCamera().WorldToScreenPoint(worldPoint);
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        return GetCamera().ScreenToWorldPoint(screenPoint);
    }


    public bool ScreenPointToWroldPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, GetCamera(), out worldPoint);
    }

    private Camera GetCamera()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        return _camera;
    }
}
