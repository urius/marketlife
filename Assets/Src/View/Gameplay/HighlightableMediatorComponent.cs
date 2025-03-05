using Src.Common;
using Src.Common_Utils;
using UnityEngine;

namespace Src.View.Gameplay
{
    public class HighlightableMediatorComponent
    {
        private readonly ScreenCalculator _screenCalculator;
        private readonly MouseDataProvider _mouseDataProvider;

        //
        private Vector2[] _projectedBoundPoints;

        public HighlightableMediatorComponent()
        {
            _screenCalculator = ScreenCalculator.Instance;
            _mouseDataProvider = MouseDataProvider.Instance;
        }

        public void UpdateBoundPoints(Vector3[] boundWorldPoints)
        {
            _projectedBoundPoints = new Vector2[boundWorldPoints.Length];
            for (var i = 0; i < boundWorldPoints.Length; i++)
            {
                var planePoint = _screenCalculator.WorldToPlaneProjection(boundWorldPoints[i]);
                _projectedBoundPoints[i] = new Vector2(planePoint.x, planePoint.y);
            }
        }

        public bool CheckMousePoint()
        {
            var result = false;
            if (_mouseDataProvider.IsMouseOverGameView == true
                && _projectedBoundPoints != null
                && _projectedBoundPoints.Length > 2)
            {
                var mouseOnPlanePosition = _mouseDataProvider.MousePlanePosition;
                result = new Vector2(mouseOnPlanePosition.x, mouseOnPlanePosition.y).IsInsidePolygon(_projectedBoundPoints);
            }
            return result;
        }
    }
}
