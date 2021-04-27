using System.Collections;
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

        return new ShopModel(dto.data.uid, designModel, shopObjects);
    }

    private static Dictionary<Vector2Int, ShopObjectBase> ToObjectsModel(string objects)
    {
        var result = new Dictionary<Vector2Int, ShopObjectBase>();
        var objectsConvertedDto = JsonConvert.DeserializeObject<Dictionary<string, string>>(objects);

        foreach (var kvp in objectsConvertedDto)
        {
            var coords = ParseCoords(kvp.Key);
            var paramsStr = kvp.Value.Split('|');
            result[coords] = CreateShopObject(coords, paramsStr);
        }

        return result;
    }

    private static ShopObjectBase CreateShopObject(Vector2Int coords, string[] paramsStr)
    {
        var splitted1StParam = paramsStr[0].Split('_');
        var typeId = splitted1StParam[0];
        var level = int.Parse(splitted1StParam[1]);
        var angle = int.Parse(paramsStr[1]);
        var side = SideHelper.GetSideFromAngle(angle);
        var objectParamsShort = paramsStr[2];

        ShopObjectBase result = typeId switch
        {
            "s" => new ShopObjectModelFactory().CreateShelf(level, coords, side, ToProducts(objectParamsShort)),
            "cd" => new ShopObjectModelFactory().CreateCashDesk(level, coords, side),
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

    private static ProductModel[] ToProducts(string objectParamsShort)
    {
        var result = new List<ProductModel>();
        var splitted = objectParamsShort.Split(',');
        foreach (var productStr in splitted)
        {
            if (productStr.Contains('p') && productStr.Contains('x'))
            {
                result.Add(ToProduct(productStr));
            }
        }

        return result.ToArray();
    }

    private static ProductModel ToProduct(string paramsShort)
    {
        var splittedByX = paramsShort.Split('x');
        var productId = int.Parse(splittedByX[0].Split('p')[1]);
        var time = 0;
        int amount;
        if (splittedByX[1].Contains("d"))
        {
            var splitedByD = splittedByX[1].Split('d');
            amount = int.Parse(splitedByD[0]);
            time = int.Parse(splitedByD[1]);
        }
        else
        {
            amount = int.Parse(splittedByX[1]);
        }

        return new ProductModel(productId, amount, time);
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
