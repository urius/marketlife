using System;
using UnityEngine;

public class GridCalculator
{
    public static GridCalculator Instance => _instance.Value;
    private static Lazy<GridCalculator> _instance = new Lazy<GridCalculator>();

    private Grid _grid;
    private float _cellSize;

    public void SetGrid(Grid grid)
    {
        _grid = grid;
        _cellSize = _grid.cellSize.x;
    }

    public Vector3 CellToWorld(Vector2Int cellCoords)
    {
        return _grid.GetCellCenterWorld(new Vector3Int(-cellCoords.y, -cellCoords.x, 0));
    }

    public Vector2Int WorldToCell(Vector3 worldCoords)
    {
        var cellCoordsRaw = _grid.WorldToCell(worldCoords);
        return new Vector2Int(-cellCoordsRaw.y, -cellCoordsRaw.x);
    }

    public Vector3 GetCellLeftCorner(Vector2Int cellCoords)
    {
        var result = CellToWorld(cellCoords);
        result.x -= _cellSize * 0.5f;
        return result;
    }

    public Vector3 GetCellRightCorner(Vector2Int cellCoords)
    {
        var result = CellToWorld(cellCoords);
        result.x += _cellSize * 0.5f;
        return result;
    }
}
