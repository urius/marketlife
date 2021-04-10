using UnityEngine;

public class GridCalculator
{
    public static GridCalculator Instance => GetOrCreateInstance();
    private static GridCalculator _instance;

    private static GridCalculator GetOrCreateInstance()
    {
        if (_instance == null)
        {
            _instance = new GridCalculator();
        }

        return _instance;
    }

    private Grid _grid;
    private float _cellSize;

    public void SetGrid(Grid grid)
    {
        _grid = grid;
        _cellSize = _grid.cellSize.x;
    }

    public Vector3 CellToWord(Vector2Int cellCoords)
    {
        return _grid.GetCellCenterWorld(new Vector3Int(cellCoords.y, cellCoords.x, 0));
    }

    public Vector2Int WorldToCell(Vector3 worldCoords)
    {
        var cellCoordsRaw = _grid.WorldToCell(worldCoords);
        return new Vector2Int(cellCoordsRaw.y, cellCoordsRaw.x);
    }

    public Vector3 GetCellLeftCorner(Vector2Int cellCoords)
    {
        var result = _grid.GetCellCenterWorld(new Vector3Int(cellCoords.y, cellCoords.x, 0));
        result.x += _cellSize * 0.5f;
        return result;
    }

    public Vector3 GetCellRightCorner(Vector2Int cellCoords)
    {
        var result = _grid.GetCellCenterWorld(new Vector3Int(cellCoords.y, cellCoords.x, 0));
        result.x -= _cellSize * 0.5f;
        return result;
    }
}
