using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopModel
{
    public event Action<ShopObjectModelBase> ShopObjectPlaced = delegate { };

    public readonly string Uid;
    public readonly ShoDesignModel ShopDesign;
    public readonly Dictionary<Vector2Int, ShopObjectModelBase> ShopObjects;
    public readonly Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)> Grid;

    public ShopModel(string uid, ShoDesignModel shopDesign, Dictionary<Vector2Int, ShopObjectModelBase> shopObjects)
    {
        Grid = new Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)>();

        Uid = uid;
        ShopDesign = shopDesign;
        ShopObjects = shopObjects;

        RefillGrid();
    }

    public void PlaceShopObject(ShopObjectModelBase shopObject)
    {
        ShopObjects[shopObject.Coords] = shopObject;
        RefillGrid();

        ShopObjectPlaced(shopObject);
    }

    public bool CanPlaceShopObject(ShopObjectModelBase shopObject)
    {
        var result = true;
        var matrix = shopObject.RotatedBuildMatrix;
        var width = matrix[0].Length;
        var pivot = new Vector2Int(width / 2, matrix.Length / 2);
        var offsetPoint = shopObject.Coords - pivot;
        for (var y = 0; y < matrix.Length; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var worldCellPoint = new Vector2Int(x, y) + offsetPoint;
                var canBuildOnCell = BuildHelper.CanBuild(matrix[y][x], GetCellBuildState(worldCellPoint));
                result &= canBuildOnCell;
            }
        }

        return result;
    }

    public bool CanPlaceFloor(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return ShopDesign.CanPlaceFloor(cellCoords, placingDecorationNumericId);
    }

    public int GetCellBuildState(Vector2Int cellCoords)
    {
        if (cellCoords.x < 0 || cellCoords.y < 0 || cellCoords.x >= ShopDesign.SizeX || cellCoords.y >= ShopDesign.SizeY) return 1;

        if (Grid.TryGetValue(cellCoords, out var cellData))
        {
            return cellData.buildState;
        }

        return 0;
    }

    private void RefillGrid()
    {
        Grid.Clear();
        foreach (var kvp in ShopObjects)
        {
            var shopObject = kvp.Value;
            var coords = shopObject.Coords;
            var buildMatrix = shopObject.RotatedBuildMatrix;
            var width = buildMatrix[0].Length;
            var pivot = new Vector2Int(width / 2, buildMatrix.Length / 2);

            for (var y = 0; y < buildMatrix.Length; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var currentCellCoords = new Vector2Int(x, y) - pivot + coords;
                    var buildState = buildMatrix[y][x];
                    if (buildState != 0)
                    {
                        Grid[currentCellCoords] = (buildState, shopObject);
                    }
                }
            }
        }
    }
}

public class ShoDesignModel
{
    public int SizeX;
    public int SizeY;

    public readonly Dictionary<Vector2Int, int> Floors;
    public readonly Dictionary<Vector2Int, int> Walls;
    public readonly Dictionary<Vector2Int, int> Windows;
    public readonly Dictionary<Vector2Int, int> Doors;

    public ShoDesignModel(int sizeX, int sizeY,
        Dictionary<Vector2Int, int> floors,
        Dictionary<Vector2Int, int> walls,
        Dictionary<Vector2Int, int> doors,
        Dictionary<Vector2Int, int> windows)
    {
        SizeX = sizeX;
        SizeY = sizeY;

        Floors = floors;
        Walls = walls;
        Doors = doors;
        Windows = windows;
    }

    public bool CanPlaceFloor(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return cellCoords.x >= 0
            && cellCoords.y >= 0
            && cellCoords.x < SizeX
            && cellCoords.y < SizeY
            && Floors[cellCoords] != placingDecorationNumericId;
    }
}

public enum ShopDecorationObjectType
{
    Undefined,
    Floor,
    Wall,
    Window,
    Door,
}