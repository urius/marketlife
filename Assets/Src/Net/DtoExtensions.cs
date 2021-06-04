using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class DtoExtensions
{
    public static ShopModel ToShopModel(this GetDataResponseDto dto)
    {
        var designModel = ToDesignModel(dto.data.design);
        var shopObjects = ToObjectsModel(dto.data.objects);
        var warehouseModel = ToWarehouseModel(dto.data.warehouse);
        var shopProgress = ToProgressModel(dto.data);

        return new ShopModel(dto.data.uid, designModel, shopObjects, shopProgress, warehouseModel);
    }

    private static ShopWarehouseModel ToWarehouseModel(string warehouseStr)
    {
        var warehouseConvertedDto = JsonConvert.DeserializeObject<Dictionary<string, string>>(warehouseStr);
        var volume = int.Parse(warehouseConvertedDto["volume"]);
        var size = int.Parse(warehouseConvertedDto["size"]);
        var products = warehouseConvertedDto["items"].Split('|')
            .Select(ToProductModel)
            .ToArray();
        var result = new ShopWarehouseModel(volume, size);
        for (var i = 0; i < size; i++)
        {
            if (i < products.Length)
            {
                result.Slots[i].SetProduct(products[i]);
            }
        }

        return result;
    }

    private static ProductModel ToProductModel(string productStr)
    {
        if (string.IsNullOrEmpty(productStr) || productStr.IndexOf('p') < 0) return null;
        var splittedByD = productStr.Split('d');
        int deliverTime = 0;
        if (splittedByD.Length > 1)
        {
            deliverTime = int.Parse(splittedByD[1]);
        }
        var splittedByX = splittedByD[0].Split('x');
        int amount = 1;
        if (splittedByX.Length > 1)
        {
            amount = int.Parse(splittedByX[1]);
        }
        var numericId = int.Parse(splittedByX[0].Split('p')[1]);
        var configsProvider = GameConfigManager.Instance.MainConfig;
        return new ProductModel(configsProvider.GetProductConfigByNumericId(numericId), amount, deliverTime);
    }

    private static ProductModel[] ToProducts(string objectParamsShort)
    {
        var result = new List<ProductModel>();
        var splitted = objectParamsShort.Split(',');
        foreach (var productStr in splitted)
        {
            if (productStr.Contains('p') && productStr.Contains('x'))
            {
                result.Add(ToProductModel(productStr));
            }
        }

        return result.ToArray();
    }

    private static ShopProgressModel ToProgressModel(GetDataResponseDataDto data)
    {
        return new ShopProgressModel(
            int.Parse(data.cash),
            int.Parse(data.gold),
            int.Parse(data.exp),
            int.Parse(data.level));
    }

    private static Dictionary<Vector2Int, ShopObjectModelBase> ToObjectsModel(string objects)
    {
        var result = new Dictionary<Vector2Int, ShopObjectModelBase>();
        var objectsConvertedDto = JsonConvert.DeserializeObject<Dictionary<string, string>>(objects);

        foreach (var kvp in objectsConvertedDto)
        {
            var coords = ParseCoords(kvp.Key);
            var paramsStr = kvp.Value.Split('|');
            result[coords] = CreateShopObject(coords, paramsStr);
        }

        return result;
    }

    private static ShopObjectModelBase CreateShopObject(Vector2Int coords, string[] paramsStr)
    {
        var splitted1StParam = paramsStr[0].Split('_');
        var typeId = splitted1StParam[0];
        var numericId = int.Parse(splitted1StParam[1]);
        var angle = int.Parse(paramsStr[1]);
        var side = SideHelper.GetSideFromAngle(angle);
        var objectParamsShort = paramsStr[2];

        ShopObjectModelBase result = typeId switch
        {
            "s" => new ShopObjectModelFactory().CreateShelf(numericId, coords, side, ToProducts(objectParamsShort)),
            "cd" => new ShopObjectModelFactory().CreateCashDesk(numericId, coords, side),
            _ => throw new System.ArgumentOutOfRangeException(typeId, $"typeId {typeId} is not supported"),
        };
        return result;
    }

    private static ShoDesignModel ToDesignModel(string design)
    {
        var designConvertedDto = JsonConvert.DeserializeObject<DesignConvertedDto>(design);

        var sizeX = designConvertedDto.size_x;
        var sizeY = designConvertedDto.size_y;

        var convertedFloors = ConvertFloors(sizeX, sizeY, designConvertedDto.floors);
        var convertedWalls = ConvertWalls(sizeX, sizeY, designConvertedDto.walls);
        var convertedDoors = ConvertDoors(designConvertedDto.doors);
        var convertedWindows = ConvertWindows(designConvertedDto.windows);

        return new ShoDesignModel(sizeX, sizeY, convertedFloors, convertedWalls, convertedDoors, convertedWindows);
    }

    private static Dictionary<Vector2Int, int> ConvertDoors(Dictionary<string, string> rawDoors)
    {
        var result = new Dictionary<Vector2Int, int>();
        foreach (var kvp in rawDoors)
        {
            var coords = ParseCoords(kvp.Key);
            result[coords] = int.Parse(kvp.Value);
        }

        return result;
    }

    private static Dictionary<Vector2Int, int> ConvertWindows(Dictionary<string, string> rawWindows)
    {
        var result = new Dictionary<Vector2Int, int>();
        foreach (var kvp in rawWindows)
        {
            var coords = ParseCoords(kvp.Key);
            result[coords] = ParseIntItem("wnd_", kvp.Value);
        }

        return result;
    }

    private static Dictionary<Vector2Int, int> ConvertWalls(int sizeX, int sizeY, Dictionary<string, string> rawWalls)
    {
        var result = new Dictionary<Vector2Int, int>();
        var stdItem = 1;
        if (rawWalls.TryGetValue("std", out var rawStdItem))
        {
            stdItem = ParseIntItem("Wall", rawStdItem);
        }

        for (var ix = 0; ix < sizeX; ix++)
        {
            result[new Vector2Int(ix, -1)] = stdItem;
        }

        for (var iy = 0; iy < sizeY; iy++)
        {
            result[new Vector2Int(-1, iy)] = stdItem;
        }

        foreach (var kvp in rawWalls)
        {
            if (kvp.Key != "std")
            {
                var coords = ParseCoords(kvp.Key);
                result[coords] = ParseIntItem("w_", kvp.Value);
            }
        }

        return result;
    }

    private static Dictionary<Vector2Int, int> ConvertFloors(int sizeX, int sizeY, Dictionary<string, string> rawData)
    {
        var result = new Dictionary<Vector2Int, int>();

        var stdItem = 1;
        if (rawData.TryGetValue("std", out var rawStdItem))
        {
            stdItem = ParseIntItem("Floor", rawStdItem);
        }

        for (var ix = 0; ix < sizeX; ix++)
        {
            for (var iy = 0; iy < sizeY; iy++)
            {
                result[new Vector2Int(ix, iy)] = stdItem;
            }
        }

        foreach (var kvp in rawData)
        {
            if (kvp.Key != "std")
            {
                var coords = ParseCoords(kvp.Key);
                result[coords] = ParseIntItem("f_", kvp.Value);
            }
        }

        return result;
    }

    private static Vector2Int ParseCoords(string coordsStr)
    {
        var result = new Vector2Int(-1, -1);

        if (coordsStr.IndexOf('y') != -1)
        {
            result.y = ParseIntItem("y", coordsStr, out var splittedCoordsStr);
            if (coordsStr.IndexOf("x") != -1)
            {
                result.x = ParseIntItem("x", splittedCoordsStr[0]);
            }
        }
        else if (coordsStr.IndexOf("x") != -1)
        {
            result.x = ParseIntItem("x", coordsStr);
        }

        return result;
    }

    private static int ParseIntItem(string prefix, string rawItemData)
    {
        return ParseIntItem(prefix, rawItemData, out var _);
    }

    private static int ParseIntItem(string prefix, string rawItemData, out string[] splitResult)
    {
        var separators = new string[] { prefix };
        splitResult = rawItemData.Split(separators, System.StringSplitOptions.None);
        return int.Parse(splitResult[1]);
    }
}

public struct DesignConvertedDto
{
    public int size_x;
    public int size_y;
    public Dictionary<string, string> floors;
    public Dictionary<string, string> doors;
    public Dictionary<string, string> windows;
    public Dictionary<string, string> walls;
}
