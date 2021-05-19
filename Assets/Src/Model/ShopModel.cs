using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopModel
{
    public event Action<ShopObjectModelBase> ShopObjectPlaced = delegate { };

    public readonly string Uid;
    public readonly ShopProgressModel ProgressModel;
    public readonly ShoDesignModel ShopDesign;
    public readonly Dictionary<Vector2Int, ShopObjectModelBase> ShopObjects;
    public readonly Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)> Grid;

    public ShopModel(string uid, ShoDesignModel shopDesign, Dictionary<Vector2Int, ShopObjectModelBase> shopObjects, ShopProgressModel progressModel)
    {
        Grid = new Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)>();

        Uid = uid;
        ShopDesign = shopDesign;
        ShopObjects = shopObjects;
        ProgressModel = progressModel;

        RefillGrid();
    }

    public bool CanSpendMoney(string price)
    {
        return CanSpendMoney(Price.FromString(price));
    }

    public bool CanSpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.CanSpendGold(price.Value) : ProgressModel.CanSpendCash(price.Value);
    }

    public bool TrySpendMoney(string price)
    {
        return TrySpendMoney(Price.FromString(price));
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.TrySpendGold(price.Value) : ProgressModel.TrySpendCash(price.Value);
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

    public bool CanPlaceFloor(Vector2Int cellCoords, int numericId)
    {
        return ShopDesign.CanPlaceFloor(cellCoords, numericId);
    }

    public bool TryPlaceFloor(Vector2Int cellCoords, int floorNumericId)
    {
        var canPlace = ShopDesign.CanPlaceFloor(cellCoords, floorNumericId);
        if (canPlace)
        {
            ShopDesign.PlaceFloor(cellCoords, floorNumericId);
        }

        return canPlace;
    }

    public bool CanPlaceWall(Vector2Int cellCoords, int numericId)
    {
        return ShopDesign.CanPlaceWall(cellCoords, numericId);
    }

    public bool TryPlaceWall(Vector2Int cellCoords, int floorNumericId)
    {
        var canPlace = ShopDesign.CanPlaceWall(cellCoords, floorNumericId);
        if (canPlace)
        {
            ShopDesign.PlaceWall(cellCoords, floorNumericId);
        }

        return canPlace;
    }

    public bool CanPlaceWindow(Vector2Int cellCoords, int numericId)
    {
        return ShopDesign.CanPlaceWindow(cellCoords, numericId);
    }

    public bool TryPlaceWindow(Vector2Int cellCoords, int floorNumericId)
    {
        var canPlace = ShopDesign.CanPlaceWindow(cellCoords, floorNumericId);
        if (canPlace)
        {
            ShopDesign.PlaceWindow(cellCoords, floorNumericId);
        }

        return canPlace;
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
    public event Action<Vector2Int, int> FloorChanged = delegate { };
    public event Action<Vector2Int, int> WallChanged = delegate { };
    public event Action<Vector2Int, int> WindowChanged = delegate { };

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

    public bool CanPlaceWall(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return IsWallCoords(cellCoords)
            && Walls[cellCoords] != placingDecorationNumericId;
    }

    public bool CanPlaceWindow(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return IsWallCoords(cellCoords)
            && (!Windows.ContainsKey(cellCoords) || Windows[cellCoords] != placingDecorationNumericId);
    }

    public void PlaceFloor(Vector2Int cellCoords, int floorNumericId)
    {
        Floors[cellCoords] = floorNumericId;
        FloorChanged(cellCoords, floorNumericId);
    }

    public void PlaceWall(Vector2Int cellCoords, int wallNumericId)
    {
        Walls[cellCoords] = wallNumericId;
        WallChanged(cellCoords, wallNumericId);
    }

    public void PlaceWindow(Vector2Int cellCoords, int windowNumericId)
    {
        Windows[cellCoords] = windowNumericId;
        WindowChanged(cellCoords, windowNumericId);
    }

    private bool IsWallCoords(Vector2Int cellCoords)
    {
        return (cellCoords.x == -1 && cellCoords.y >= 0 && cellCoords.y < SizeY)
            || (cellCoords.y == -1 && cellCoords.x >= 0 && cellCoords.x < SizeX);
    }

}

public class ShopProgressModel
{
    public event Action<int, int> CashChanged = delegate { };
    public event Action<int, int> GoldChanged = delegate { };
    public event Action<int, int> ExpChanged = delegate { };
    public event Action<int, int> LevelChanged = delegate { };

    public int Cash => Decode(_cashEncoded);
    public int Gold => Decode(_goldEncoded);
    public int ExpAmount => Decode(_expEncoded);
    public int Level => Decode(_levelEncoded);

    private string _cashEncoded;
    private string _goldEncoded;
    private string _expEncoded;
    private string _levelEncoded;

    public ShopProgressModel(int cash, int gold, int expAmount, int level)
    {
        _cashEncoded = Encode(cash);
        _goldEncoded = Encode(gold);
        _expEncoded = Encode(expAmount);
        _levelEncoded = Encode(level);
    }

    public void SetCash(int newValue)
    {
        var valueBefore = Cash;
        _cashEncoded = Encode(newValue);
        CashChanged(valueBefore, newValue);
    }

    public bool CanSpendCash(int spendAmount)
    {
        var currentValue = Cash;
        return currentValue >= spendAmount;
    }

    public bool TrySpendCash(int spendAmount)
    {
        var currentValue = Cash;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetCash(currentValue);
            return true;
        }
        return false;
    }

    public void SetGold(int newValue)
    {
        var valueBefore = Gold;
        _goldEncoded = Encode(newValue);
        GoldChanged(valueBefore, newValue);
    }

    public bool CanSpendGold(int spendAmount)
    {
        var currentValue = Gold;
        return currentValue >= spendAmount;
    }

    public bool TrySpendGold(int spendAmount)
    {
        var currentValue = Gold;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetGold(currentValue);
            return true;
        }
        return false;
    }

    public void SetExp(int newValue)
    {
        var valueBefore = ExpAmount;
        _expEncoded = Encode(newValue);
        ExpChanged(valueBefore, newValue);
    }

    public void SetLevel(int newValue)
    {
        var valueBefore = Level;
        _levelEncoded = Encode(newValue);
        LevelChanged(valueBefore, newValue);
    }

    private string Encode(int input)
    {
        return Base64Helper.Base64Encode(input.ToString());
    }

    private int Decode(string base64Input)
    {
        return int.Parse(Base64Helper.Base64Decode(base64Input));
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