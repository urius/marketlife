using System;
using Src.View.Gameplay;
using UnityEngine;

namespace Src.Common
{
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

        public bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector3 worldPoint)
        {
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, GetCamera(), out worldPoint);
        }

        public Vector3 CellToScreenPoint(Vector2Int cellCoords)
        {
            var worldPoint = GridCalculator.Instance.CellToWorld(cellCoords);
            return WorldToScreenPoint(worldPoint);
        }

        public Vector3 ScreenPointToPlaneWorldPoint(Vector2 screenPoint)
        {
            var mouseWorldPoint = GetCamera().ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y));
            return WorldToPlaneProjection(mouseWorldPoint);
        }

        public Vector3 WorldToPlaneProjection(Vector3 worldPoint)
        {
            var cameraTransform = GetCamera().transform;
            var rotationX = Mathf.Deg2Rad * cameraTransform.rotation.eulerAngles.x;
            var distance = (float)(Math.Abs(worldPoint.z) / Math.Cos(rotationX));

            var result = worldPoint + cameraTransform.forward * distance;

            return result;
        }

        public Vector3 PlaneToCameraProjection(Vector3 worldPoint)
        {
            var cameraTransform = GetCamera().transform;
            var rotationX = Mathf.Deg2Rad * cameraTransform.rotation.eulerAngles.x;
            var distance = (float)(Math.Abs(cameraTransform.position.z) / Math.Cos(rotationX));

            var result = worldPoint - cameraTransform.forward * distance;

            return result;
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
}
