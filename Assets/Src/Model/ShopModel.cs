using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopModel
{
    public event Action<ShopObjectModelBase> ShopObjectPlaced = delegate { };
    public event Action<ShopObjectModelBase> ShopObjectRemoved = delegate { };
    public event Action<Vector2Int> UnwashAdded = delegate { };
    public event Action<Vector2Int> UnwashRemoved = delegate { };
    public event Action ShopObjectsChanged = delegate { };
    public event Action<float> MoodChanged = delegate { };

    public readonly ShopPersonalModel PersonalModel;
    public readonly ShopWarehouseModel WarehouseModel;
    public readonly ShoDesignModel ShopDesign;
    public readonly Dictionary<Vector2Int, ShopObjectModelBase> ShopObjects;
    public readonly Dictionary<Vector2Int, int> Unwashes;
    public readonly Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)> Grid;

    private float _moodMultiplier;
    private int _totalFreeShelfSlots;
    private int _totalUsedShelfSlots;

    public ShopModel(
        ShoDesignModel shopDesign,
        Dictionary<Vector2Int, ShopObjectModelBase> shopObjects,
        Dictionary<Vector2Int, int> unwashes,
        ShopPersonalModel personalModel,
        ShopWarehouseModel warehouseModel)
    {
        Grid = new Dictionary<Vector2Int, (int buildState, ShopObjectModelBase reference)>();

        ShopDesign = shopDesign;
        ShopObjects = shopObjects;
        Unwashes = unwashes;
        PersonalModel = personalModel;
        WarehouseModel = warehouseModel;

        RecalculateMood();

        RefillGrid();

        Activate();
    }

    public float MoodMultiplier => _moodMultiplier;
    public int SlotsFullnessPercent => (int)(100 * ((float)_totalUsedShelfSlots / (_totalUsedShelfSlots + _totalFreeShelfSlots)));
    public int ClarityPercent => (int)(100 * (1f - (float)Unwashes.Count / (ShopDesign.SizeX * ShopDesign.SizeY)));

    public bool CanPlaceUnwash(Vector2Int coords)
    {
        if (Grid.TryGetValue(coords, out var gridItem))
        {
            if (gridItem.buildState > 0)
            {
                return false;
            }
        }

        return !Unwashes.ContainsKey(coords);
    }

    public bool AddRandomUnwash()
    {
        var random = new System.Random();
        var coords = new Vector2Int(-1, -1);
        foreach (var kvp in Grid)
        {
            if (kvp.Value.buildState <= 0 && Unwashes.ContainsKey(kvp.Key) == false)
            {
                coords = kvp.Key;
                if (random.NextDouble() < 0.2f) break;
            }
        }
        if (coords != new Vector2Int(-1, -1))
        {
            return AddUnwash(coords, random.Next(1, 4));
        }
        return false;
    }

    public bool AddUnwash(Vector2Int coords, int unwashIdIndex)
    {
        if (CanPlaceUnwash(coords))
        {
            Unwashes[coords] = unwashIdIndex;
            UnwashAdded(coords);
            UpdateMood();
            return true;
        }
        return false;
    }

    public bool RemoveUnwash(Vector2Int coords)
    {
        var result = Unwashes.Remove(coords);
        if (result)
        {
            UnwashRemoved(coords);
            UpdateMood();
        }

        return result;
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
        if (shopObject.Type == ShopObjectType.Shelf)
        {
            var shelfModel = shopObject as ShelfModel;
            SubscribeOnShelfModel(shelfModel);
            foreach (var slot in shelfModel.Slots)
            {
                if (slot.HasProduct)
                {
                    _totalUsedShelfSlots++;
                }
                else
                {
                    _totalFreeShelfSlots++;
                }
            }
            UpdateMood();
        }

        RefillGrid();

        ShopObjectPlaced(shopObject);
    }

    public bool CanRotateShopObject(ShopObjectModelBase shopObject, int deltaSide)
    {
        var result = false;
        if (RemoveFromGrid(shopObject) > 0)
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
            if (shopObject.Type == ShopObjectType.Shelf)
            {
                var shelfModel = shopObject as ShelfModel;
                UnsubscribeFromShelfModel(shelfModel);
                foreach (var slot in shelfModel.Slots)
                {
                    if (slot.HasProduct)
                    {
                        _totalUsedShelfSlots--;
                    }
                    else
                    {
                        _totalFreeShelfSlots--;
                    }
                }
                UpdateMood();
            }

            RefillGrid();
            ShopObjectRemoved(shopObject);
        }
        else
        {
            throw new InvalidOperationException($"RemoveShopObject: object {shopObject.Type} with coords {shopObject.Coords} is not placed on shop");
        }
    }

    public void RotateShopObject(ShopObjectModelBase shopObject, int deltaSide)
    {
        if (RemoveFromGrid(shopObject) > 0)
        {
            shopObject.Side += deltaSide;
            RefillGrid();
        }
        else
        {
            throw new InvalidOperationException($"RotateShopObject: object {shopObject.Type} failed to remove from grid, coords: {shopObject.Coords}, shop");
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

    public (ProductModel[] Products, int TotalVolume, int UsedVolume) GetAllProductsInfo()
    {
        var totalVolume = 0;
        var usedVolume = 0;
        var productsDictionary = new Dictionary<ProductConfig, int>();
        foreach (var kvp in ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = kvp.Value as ShelfModel;
                foreach (var slot in shelfModel.Slots)
                {
                    totalVolume += slot.Volume;
                    if (slot.HasProduct)
                    {
                        var productConfig = slot.Product.Config;
                        usedVolume += slot.Product.Amount * productConfig.Volume;
                        if (productsDictionary.ContainsKey(productConfig))
                        {
                            productsDictionary[productConfig] += slot.Product.Amount;
                        }
                        else
                        {
                            productsDictionary[productConfig] = slot.Product.Amount;
                        }
                    }
                }
            }
        }

        var productsList = new List<ProductModel>(productsDictionary.Count);
        foreach (var kvp in productsDictionary)
        {
            productsList.Add(new ProductModel(kvp.Key, kvp.Value));
        }

        return (productsList.ToArray(), totalVolume, usedVolume);
    }

    public Dictionary<ProductConfig, int> RemoveProducts(IReadOnlyDictionary<ProductConfig, int> productsToRemove)
    {
        var restProductsToRemove = new Dictionary<ProductConfig, int>(productsToRemove.Count);
        foreach (var inputProductToRemove in productsToRemove)
        {
            restProductsToRemove[inputProductToRemove.Key] = inputProductToRemove.Value;
        }

        foreach (var shopObject in ShopObjects)
        {
            if (shopObject.Value.Type == ShopObjectType.Shelf)
            {
                var shelfSlots = (shopObject.Value as ShelfModel).Slots;
                foreach (var slot in shelfSlots)
                {
                    if (slot.HasProduct)
                    {
                        var productConfig = slot.Product.Config;
                        if (restProductsToRemove.ContainsKey(productConfig)
                            && restProductsToRemove[productConfig] > 0)
                        {
                            var amountToRemove = Math.Min(restProductsToRemove[productConfig], slot.Product.Amount);
                            if (amountToRemove > 0)
                            {
                                slot.ChangeProductAmount(-amountToRemove);
                                restProductsToRemove[productConfig] -= amountToRemove;
                            }
                        }
                    }
                }
            }
        }

        return restProductsToRemove;
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

    public bool IsBuiltOnCell(Vector2Int cellCoords)
    {
        return (Grid.ContainsKey(cellCoords) && Grid[cellCoords].buildState > 0);
    }

    private void Activate()
    {
        foreach (var kvp in ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                SubscribeOnShelfModel(kvp.Value as ShelfModel);
            }
        }
        ShopDesign.SizeXChanged += OnSizeXChanged;
        ShopDesign.SizeYChanged += OnSizeYChanged;
    }

    private void OnSizeYChanged(int previousValue, int currentValue)
    {
        UpdateMood();
    }

    private void OnSizeXChanged(int previousValue, int currentValue)
    {
        UpdateMood();
    }

    private void RecalculateMood()
    {
        _totalUsedShelfSlots = 0;
        _totalFreeShelfSlots = 0;
        foreach (var kvp in ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = kvp.Value as ShelfModel;
                foreach (var slot in shelfModel.Slots)
                {
                    if (slot.HasProduct)
                    {
                        _totalUsedShelfSlots++;
                    }
                    else
                    {
                        _totalFreeShelfSlots++;
                    }
                }
            }
        }
        UpdateMood();
    }

    private void SubscribeOnShelfModel(ShelfModel shelfModel)
    {
        shelfModel.ProductIsSetOnSlot += OnShelfProductIsSetOnSlot;
        shelfModel.ProductRemovedFromSlot += OnShelfProductRemovedFromSlot;
    }

    private void UnsubscribeFromShelfModel(ShelfModel shelfModel)
    {
        shelfModel.ProductIsSetOnSlot -= OnShelfProductIsSetOnSlot;
        shelfModel.ProductRemovedFromSlot -= OnShelfProductRemovedFromSlot;
    }

    private void OnShelfProductIsSetOnSlot(ShelfModel shelfModel, int slotIndex)
    {
        if (shelfModel.Slots[slotIndex].HasProduct && shelfModel.Slots[slotIndex].Product.Amount > 0)
        {
            _totalUsedShelfSlots++;
            _totalFreeShelfSlots--;
            UpdateMood();
        }
    }

    private void OnShelfProductRemovedFromSlot(ShelfModel shelfModel, int slotIndex, ProductModel productModel)
    {
        _totalUsedShelfSlots--;
        _totalFreeShelfSlots++;
        UpdateMood();
    }

    private void UpdateMood()
    {
        var totalShopSquare = ShopDesign.SizeX * ShopDesign.SizeY;
        var unwashesCount = Unwashes.Count;
        var moodMultiplierBefore = _moodMultiplier;
        _moodMultiplier = CalculationHelper.CalculateMood(_totalUsedShelfSlots, _totalFreeShelfSlots + _totalUsedShelfSlots, unwashesCount, totalShopSquare);
        MoodChanged(_moodMultiplier - moodMultiplierBefore);
    }

    private void RefillGrid()
    {
        Grid.Clear();
        foreach (var kvp in ShopObjects)
        {
            var shopObject = kvp.Value;
            AddToGrid(shopObject);
        }
        ShopObjectsChanged();
    }

    private void AddToGrid(ShopObjectModelBase shopObject)
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
    }

    private int RemoveFromGrid(ShopObjectModelBase shopObject)
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

        return result;
    }
}

public class ShoDesignModel
{
    public event Action<Vector2Int, int> FloorChanged = delegate { };
    public event Action<Vector2Int, int> WallChanged = delegate { };
    public event Action<Vector2Int, int> WindowChanged = delegate { };
    public event Action<Vector2Int, int> DoorChanged = delegate { };
    public event Action<int, int> SizeXChanged = delegate { };
    public event Action<int, int> SizeYChanged = delegate { };

    public readonly Dictionary<Vector2Int, int> Floors;
    public readonly Dictionary<Vector2Int, int> Walls;
    public readonly Dictionary<Vector2Int, int> Windows;
    public readonly Dictionary<Vector2Int, int> Doors;

    public ShoDesignModel(int sizeX, int sizeY,
        Dictionary<Vector2Int, int> floors,
        Dictionary<Vector2Int, int> walls,
        Dictionary<Vector2Int, int> windows,
        Dictionary<Vector2Int, int> doors)
    {
        SizeX = sizeX;
        SizeY = sizeY;

        Floors = floors;
        Walls = walls;
        Windows = windows;
        Doors = doors;
    }

    public int SizeX { get; private set; }
    public int SizeY { get; private set; }

    public void SetSizeX(int value)
    {
        var prevValue = SizeX;
        SizeX = value;
        for (var x = prevValue; x < SizeX; x++)
        {
            Walls[new Vector2Int(x, -1)] = 1;
            for (var y = 0; y < SizeY; y++)
            {
                Floors[new Vector2Int(x, y)] = 1;
            }
        }
        SizeXChanged(prevValue, SizeX);
    }

    public void SetSizeY(int value)
    {
        var prevValue = SizeY;
        SizeY = value;

        for (var y = prevValue; y < SizeY; y++)
        {
            Walls[new Vector2Int(-1, y)] = 1;
            for (var x = 0; x < SizeX; x++)
            {
                Floors[new Vector2Int(x, y)] = 1;
            }
        }
        SizeYChanged(prevValue, SizeY);
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

    public bool IsCellInside(Vector2Int cellCoords)
    {
        return cellCoords.x >= 0 && cellCoords.y >= 0 && cellCoords.x < SizeX && cellCoords.y < SizeY;
    }

    private bool IsWallCoords(Vector2Int cellCoords)
    {
        return (cellCoords.x == -1 && cellCoords.y >= 0 && cellCoords.y < SizeY)
            || (cellCoords.y == -1 && cellCoords.x >= 0 && cellCoords.x < SizeX);
    }
}

public class ShopWarehouseModel
{
    public event Action<int> SlotProductIsSet = delegate { };
    public event Action<int, ProductModel> SlotProductRemoved = delegate { };
    public event Action<int, int> SlotProductAmountChanged = delegate { };
    public event Action<int> VolumeAdded = delegate { };
    public event Action<int> SlotsAdded = delegate { };

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

    public void AddSlots(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var newSlot = new ProductSlotModel(Size, Volume);
            SubscribeOnSlot(newSlot);
            _slots.Add(newSlot);
        }
        SlotsAdded(amount);
    }

    public Dictionary<ProductConfig, int> RemoveDeliveredProducts(IReadOnlyDictionary<ProductConfig, int> productsToRemove, int targetTimestamp)
    {
        var restProductsToRemove = new Dictionary<ProductConfig, int>(productsToRemove.Count);
        foreach (var inputProductToRemove in productsToRemove)
        {
            restProductsToRemove[inputProductToRemove.Key] = inputProductToRemove.Value;
        }

        foreach (var slot in _slots)
        {
            if (slot.HasProduct && slot.Product.DeliverTime <= targetTimestamp)
            {
                var productConfig = slot.Product.Config;
                if (restProductsToRemove.ContainsKey(productConfig) && restProductsToRemove[productConfig] > 0)
                {
                    var amountToRemove = Math.Min(restProductsToRemove[productConfig], slot.Product.Amount);
                    if (amountToRemove > 0)
                    {
                        slot.ChangeProductAmount(-amountToRemove);
                        restProductsToRemove[productConfig] -= amountToRemove;
                    }
                }
            }
        }

        return restProductsToRemove;
    }

    public int GetDeliveredProductAmount(int numericId, int targetTime)
    {
        var result = 0;
        foreach (var slot in Slots)
        {
            if (slot.HasProduct
                && slot.Product.Config.NumericId == numericId
                && slot.Product.DeliverTime <= targetTime)
            {
                result += slot.Product.Amount;
            }
        }
        return result;
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
    public event Action<PersonalConfig> PersonalWorkingTimeUpdated = delegate { };

    public Dictionary<PersonalConfig, int> PersonalData => _personalWorkingTimes;
    private Dictionary<PersonalConfig, int> _personalWorkingTimes = new Dictionary<PersonalConfig, int>();

    public ShopPersonalModel()
    {
    }

    public void SetPersonalWorkingTime(PersonalConfig personalConfig, int endWorkTime)
    {
        _personalWorkingTimes[personalConfig] = endWorkTime;
        PersonalWorkingTimeUpdated(personalConfig);
    }

    public int GetEndWorkTime(PersonalConfig personalConfig)
    {
        if (_personalWorkingTimes.TryGetValue(personalConfig, out var endWorkTime))
        {
            return endWorkTime;
        }
        return 0;
    }

    public int GetMaxEndWorkTimeForPersonalType(PersonalType personalType)
    {
        var result = 0;
        foreach (var kvp in _personalWorkingTimes)
        {
            if (kvp.Key.TypeId == personalType && kvp.Value > result)
            {
                result = kvp.Value;
            }
        }
        return result;
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