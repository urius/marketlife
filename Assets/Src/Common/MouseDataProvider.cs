using System;
using UnityEngine;

namespace Src.Common
{
    public class MouseDataProvider
    {
        private static Lazy<MouseDataProvider> _instance = new Lazy<MouseDataProvider>();
        public static MouseDataProvider Instance => _instance.Value;

        public Vector2Int MouseCellCoords { get; private set; }
        public bool IsMouseOverGameView { get; private set; }
        public Vector3 MousePlanePosition { get; private set; }

        public void SetMouseCellCoords(Vector2Int newCoords, bool forceUpdate = false)
        {
            if (newCoords != MouseCellCoords || forceUpdate)
            {
                MouseCellCoords = newCoords;
                Dispatcher.Instance.MouseCellCoordsUpdated(newCoords);
            }
        }

        public void SetMouseOverGameView(bool isOverGameView)
        {
            IsMouseOverGameView = isOverGameView;
        }

        public void SetMouseProjectedOnPlaneWorldPosition(Vector3 mousePlanePosition)
        {
            MousePlanePosition = mousePlanePosition;
        }
    }
}