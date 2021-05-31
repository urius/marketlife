using System;
using UnityEngine;

public class MouseCellCoordsProvider
{
    private static Lazy<MouseCellCoordsProvider> _instance = new Lazy<MouseCellCoordsProvider>();
    public static MouseCellCoordsProvider Instance => _instance.Value;

    public Vector2Int MouseCellCoords { get; private set; }

    public void SetMouseCellCoords(Vector2Int newCoords, bool forceUpdate = false)
    {
        if (newCoords != MouseCellCoords || forceUpdate)
        {
            MouseCellCoords = newCoords;
            Dispatcher.Instance.MouseCellCoordsUpdated(newCoords);
        }
    }
}
