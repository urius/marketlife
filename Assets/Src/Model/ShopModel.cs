using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopModel
{
    public event Action<ShopObjectModelBase> ShopObjectPlaced = delegate { };
    public event Action<ShopObjectModelBase> ShopObjectRemoved = delegate { };
    public event Action ShopModelChanged = delegate { };

    public readonly string Uid;
    public readonly ShopProgressModel ProgressModel;
    public readonly ShopPersonalModel PersonalModel;
    public readonly ShopWarehouseModel WarehouseModel;
    public readonly ShoDesignModel ShopDesign;
    public readonly Dictionary<Vector2Int, ShopObjectModelBase> ShopObjects;
    public readonly Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)> Grid;

    public ShopModel(
        string uid,
        ShoDesignModel shopDesign,
        Dictionary<Vector2Int, ShopObjectModelBase> shopObjects,
        ShopProgressModel progressModel,
        ShopPersonalModel personalModel,
        ShopWarehouseModel warehouseModel)
    {
        Grid = new Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)>();

        Uid = uid;
        ShopDesign = shopDesign;
        ShopObjects = shopObjects;
        ProgressModel = progressModel;
        PersonalModel = personalModel;
        WarehouseModel = warehouseModel;

        RefillGrid();
    }

    public int GetSellPrice(Price originalPrice)
    {
        return originalPrice.IsGold ? originalPrice.Value * 1000 : (int)(originalPrice.Value * 0.5f);
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

    public void PlaceShopObject(ShopObjectModelBase shopObject)
    {
        ShopObjects[shopObject.Coords] = shopObject;
        RefillGrid();

        ShopObjectPlaced(shopObject);
    }

    public bool CanRotateShopObject(ShopObjectModelBase shopObject, int deltaSide)
    {
        var result = false;
        if (RemoveFromGrid(shopObject, true) > 0)
        {
            var originalSide = shopObject.Side;
            shopObject.Side += deltaSide;
            result = CanPlaceShopObject(shopObject);
            shopObject.Side = originalSide;
            AddToGrid(shopObject);
        }
        return result;
    }

    public void RemoveShopObject(ShopObjectModelBase shopObject)
    {
        if (ShopObjects[shopObject.Coords] == shopObject)
        {
            ShopObjects.Remove(shopObject.Coords);
            RefillGrid();
            ShopObjectRemoved(shopObject);
        }
        else
        {
            throw new InvalidOperationException($"RemoveShopObject: object {shopObject.Type} with coords {shopObject.Coords} is not placed on shop ${Uid}");
        }
    }

    public void RotateShopObject(ShopObjectModelBase shopObject, int deltaSide)
    {
        if (RemoveFromGrid(shopObject, true) > 0)
        {
            shopObject.Side += deltaSide;
            RefillGrid();
        }
        else
        {
            throw new InvalidOperationException($"RotateShopObject: object {shopObject.Type} failed to remove from grid, coords: {shopObject.Coords}, shop uid: ${Uid}");
        }
    }

    public bool CanPlaceDecoration(ShopDecorationObjectType decorationType, Vector2Int coords, int numericId)
    {
        return decorationType switch
        {
            ShopDecorationObjectType.Floor => CanPlaceFloor(coords, numericId),
            ShopDecorationObjectType.Wall => CanPlaceWall(coords, numericId),
            ShopDecorationObjectType.Window => CanPlaceWindow(coords, numericId),
            ShopDecorationObjectType.Door => CanPlaceDoor(coords, numericId),
            _ => false,
        };
    }

    public bool TryPlaceDecoration(ShopDecorationObjectType decorationType, Vector2Int coords, int numericId)
    {
        return decorationType switch
        {
            ShopDecorationObjectType.Floor => TryPlaceFloor(coords, numericId),
            ShopDecorationObjectType.Wall => TryPlaceWall(coords, numericId),
            ShopDecorationObjectType.Window => TryPlaceWindow(coords, numericId),
            ShopDecorationObjectType.Door => TryPlaceDoor(coords, numericId),
            _ => false,
        };
    }

    public bool TryRemoveDecoration(Vector2Int coords)
    {
        var decorationType = ShopDesign.GetDecorationType(coords);
        return decorationType switch
        {
            ShopDecorationObjectType.Window => ShopDesign.RemoveWindow(coords),
            ShopDecorationObjectType.Door => ShopDesign.RemoveDoor(coords),
            _ => false,
        };
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

    public bool CanPlaceDoor(Vector2Int cellCoords, int numericId)
    {
        return ShopDesign.CanPlaceDoor(cellCoords, numericId);
    }

    public bool TryPlaceDoor(Vector2Int cellCoords, int numericId)
    {
        var canPlace = ShopDesign.CanPlaceDoor(cellCoords, numericId);
        if (canPlace)
        {
            ShopDesign.PlaceDoor(cellCoords, numericId);
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
            AddToGrid(shopObject, true);
        }
        ShopModelChanged();
    }

    private void AddToGrid(ShopObjectModelBase shopObject, bool isSilent = false)
    {
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
                    if (Grid.ContainsKey(currentCellCoords) && Grid[currentCellCoords].buildState > 0)
                    {
                        continue;
                    }
                    Grid[currentCellCoords] = (buildState, shopObject);
                }
            }
        }

        if (isSilent == false)
        {
            ShopModelChanged();
        }
    }

    private int RemoveFromGrid(ShopObjectModelBase shopObject, bool isSilent = false)
    {
        var result = 0;
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
                    if (Grid.TryGetValue(currentCellCoords, out var shopObjectInfo))
                    {
                        if (shopObjectInfo.buildState != 0 && shopObjectInfo.reference == shopObject)
                        {
                            Grid.Remove(currentCellCoords);
                            result++;
                        }
                    }
                }
            }
        }

        if (isSilent == false && result > 0)
        {
            ShopModelChanged();
        }

        return result;
    }
}

public class ShoDesignModel
{
    public event Action<Vector2Int, int> FloorChanged = delegate { };
    public event Action<Vector2Int, int> WallChanged = delegate { };
    public event Action<Vector2Int, int> WindowChanged = delegate { };
    public event Action<Vector2Int, int> DoorChanged = delegate { };

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

    public void PlaceFloor(Vector2Int cellCoords, int floorNumericId)
    {
        Floors[cellCoords] = floorNumericId;
        FloorChanged(cellCoords, floorNumericId);
    }

    public bool CanPlaceWall(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return IsWallCoords(cellCoords)
            && Walls[cellCoords] != placingDecorationNumericId;
    }

    public void PlaceWall(Vector2Int cellCoords, int wallNumericId)
    {
        Walls[cellCoords] = wallNumericId;
        WallChanged(cellCoords, wallNumericId);
    }

    public bool CanPlaceWindow(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return IsWallCoords(cellCoords)
            && !Doors.ContainsKey(cellCoords)
            && (!Windows.ContainsKey(cellCoords) || Windows[cellCoords] != placingDecorationNumericId);
    }

    public void PlaceWindow(Vector2Int cellCoords, int windowNumericId)
    {
        Windows[cellCoords] = windowNumericId;
        WindowChanged(cellCoords, windowNumericId);
    }

    public bool RemoveWindow(Vector2Int cellCoords)
    {
        if (Windows.Remove(cellCoords))
        {
            WindowChanged(cellCoords, 0);
            return true;
        }
        return false;
    }

    public bool CanPlaceDoor(Vector2Int cellCoords, int placingDecorationNumericId)
    {
        return IsWallCoords(cellCoords)
            && !Windows.ContainsKey(cellCoords)
            && (!Doors.ContainsKey(cellCoords) || Doors[cellCoords] != placingDecorationNumericId);
    }

    public void PlaceDoor(Vector2Int cellCoords, int doorNumericId)
    {
        Doors[cellCoords] = doorNumericId;
        DoorChanged(cellCoords, doorNumericId);
    }

    public bool RemoveDoor(Vector2Int cellCoords)
    {
        if (Doors.Remove(cellCoords))
        {
            DoorChanged(cellCoords, 0);
            return true;
        }
        return false;
    }

    public ShopDecorationObjectType GetDecorationType(Vector2Int coords)
    {
        if (Doors.ContainsKey(coords))
        {
            return ShopDecorationObjectType.Door;
        }
        else if (Windows.ContainsKey(coords))
        {
            return ShopDecorationObjectType.Window;
        }
        else if (Walls.ContainsKey(coords))
        {
            return ShopDecorationObjectType.Wall;
        }
        else if (Floors.ContainsKey(coords))
        {
            return ShopDecorationObjectType.Floor;
        }

        return ShopDecorationObjectType.Undefined;
    }

    public bool IsHighlightableDecorationOn(Vector2Int coords)
    {
        var type = GetDecorationType(coords);
        return type == ShopDecorationObjectType.Window || type == ShopDecorationObjectType.Door;
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

    public void AddCash(int amount)
    {
        TrySpendCash(-amount);
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? TrySpendGold(price.Value) : TrySpendCash(price.Value);
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

    public void AddGold(int amount)
    {
        TrySpendGold(-amount);
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

public class ShopWarehouseModel
{
    public event Action<int> SlotProductIsSet = delegate { };
    public event Action<int, ProductModel> SlotProductRemoved = delegate { };
    public event Action<int, int> SlotProductAmountChanged = delegate { };
    public event Action<int> VolumeAdded = delegate { };
    public event Action SlotAdded = delegate { };

    private readonly List<ProductSlotModel> _slots;

    public ShopWarehouseModel(int volume, int size)
    {
        Volume = volume;

        _slots = new List<ProductSlotModel>(size);
        for (var i = 0; i < size; i++)
        {
            var slot = new ProductSlotModel(i, Volume);
            _slots.Add(slot);
            SubscribeOnSlot(slot);
        }
    }

    public int Volume { get; private set; }
    public int Size => _slots.Count;
    public IReadOnlyList<ProductSlotModel> Slots => _slots;

    public void AddVolume(int addedVolume)
    {
        var previousVolume = Volume;
        Volume += addedVolume;
        foreach (var slot in Slots)
        {
            slot.AddVolume(addedVolume);
        }
        VolumeAdded(addedVolume);
    }

    public void AddSlot()
    {
        var newSlot = new ProductSlotModel(Size, Volume);
        SubscribeOnSlot(newSlot);
        _slots.Add(newSlot);
        SlotAdded();
    }

    private void SubscribeOnSlot(ProductSlotModel slot)
    {
        slot.ProductIsSet += OnSlotProductSet;
        slot.ProductRemoved += OnSlotProductRemoved;
        slot.ProductAmountChanged += OnSlotProductAmountChanged;
    }

    private void OnSlotProductAmountChanged(int slotIndex, int deltaAmount)
    {
        SlotProductAmountChanged(slotIndex, deltaAmount);
    }

    private void OnSlotProductSet(int slotIndex)
    {
        SlotProductIsSet(slotIndex);
    }

    private void OnSlotProductRemoved(int slotIndex, ProductModel removedProduct)
    {
        SlotProductRemoved(slotIndex, removedProduct);
    }
}

public class ShopPersonalModel
{
    private int _endworkTimeStub;

    public ShopPersonalModel()
    {
        _endworkTimeStub = GameStateModel.Instance.ServerTime + 20;
    }

    public int GetEndWorkTime(PersonalType personalType)
    {
        //TODO: implement
        return _endworkTimeStub;
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