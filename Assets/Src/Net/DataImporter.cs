using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class DataImporter
{
    public static DataImporter Instance => _instance.Value;
    private static Lazy<DataImporter> _instance = new Lazy<DataImporter>();

    public UserModel ImportOld(GetDataOldResponseDto dto)
    {
        var designModel = ToDesignModelOld(dto.data.design);
        var shopObjects = ToObjectsModelOld(dto.data.objects);
        var warehouseModel = ToWarehouseModelOld(dto.data.warehouse);
        var shopProgress = ToProgressModelOld(dto.data);

        var shopModel = new ShopModel(
            designModel,
            shopObjects,
            new Dictionary<Vector2Int, int>(),
            new ShopPersonalModel(),
            warehouseModel);

        return new UserModel(dto.data.uid, shopProgress, shopModel, new UserStatsData(), null, new AvailableFriendShopActionsDataModel(Array.Empty<AvailableFriendShopActionData>()), new ExternalActionsModel());
    }

    public UserModel Import(GetDataResponseDto deserializedData)
    {
        var dataDto = deserializedData.data;
        var shopModel = ToShopModel(dataDto);
        var shopProgress = ToProgressModel(dataDto.progress);
        var statsData = new UserStatsData(deserializedData.first_visit_time, deserializedData.last_visit_time, deserializedData.days_play);
        var actionsDataModel = ToAvailableActionsDataModel(dataDto.actions_data);
        var externalActionsModel = ToExternalActionsModel(deserializedData.external_data);
        return new UserModel(deserializedData.uid, shopProgress, shopModel, statsData, dataDto.tutorial_steps, actionsDataModel, externalActionsModel);
    }

    private ExternalActionsModel ToExternalActionsModel(ExternalDataDto externalData)
    {
        var result = new ExternalActionsModel();
        if (externalData.actions != null)
        {
            foreach (var externalActionDataStr in externalData.actions)
            {
                var splitted = externalActionDataStr.Split(':');
                var performerId = splitted[0];
                var actionsStr = splitted[1].Split(';');
                var actions = actionsStr.Select(s => ToAction(performerId, s)).Where(a => a != null);
                foreach (var action in actions)
                {
                    result.AddAction(action);
                }
            }
        }

        return result;
    }

    private ExternalActionModelBase ToAction(string performerId, string actionStr)
    {
        var splitted = actionStr.Split('|');
        if (splitted.Length > 0)
        {
            var actionIdInt = int.Parse(splitted[0]);
            var actionId = (FriendShopActionId)actionIdInt;
            switch (actionId)
            {
                case FriendShopActionId.Take:
                    var product = ToProductModel(splitted[2]);
                    return new ExternalActionTake(performerId, ParseCoords(splitted[1]), product.Config, product.Amount);
                case FriendShopActionId.AddUnwash:
                    return new ExternalActionAddUnwash(performerId, ParseCoords(splitted[1]));
            }
        }

        return null;
    }

    private AvailableFriendShopActionsDataModel ToAvailableActionsDataModel(string[] actionsDataRaw)
    {
        var actionsDataConverted = new AvailableFriendShopActionData[actionsDataRaw != null ? actionsDataRaw.Length : 0];
        if (actionsDataRaw != null)
        {
            for (var i = 0; i < actionsDataRaw.Length; i++)
            {
                var actionDataStr = actionsDataRaw[i];
                var splitted = actionDataStr.Split('|');
                var id = (FriendShopActionId)int.Parse(splitted[0]);
                var amountRest = int.Parse(splitted[1]);
                var endCooldownTimestamp = int.Parse(splitted[2]);
                actionsDataConverted[i] = new AvailableFriendShopActionData(id, amountRest, endCooldownTimestamp);
            }
        }

        var actionsDataResult = new AvailableFriendShopActionData[AvailableFriendShopActionsDataModel.SupportedActionsCount];
        var mainConfig = GameConfigManager.Instance.MainConfig;
        for (var i = 0; i < AvailableFriendShopActionsDataModel.SupportedActionsCount; i++)
        {
            var actionId = i + 1;
            var haveAction = actionsDataConverted.Any(a => (int)a.ActionId == actionId);
            var actionData = haveAction ?
                actionsDataConverted.First(a => (int)a.ActionId == actionId) :
                new AvailableFriendShopActionData((FriendShopActionId)actionId, mainConfig.ActionDefaultAmount);
            actionsDataResult[i] = actionData;
        }

        return new AvailableFriendShopActionsDataModel(actionsDataResult);
    }

    private ShopModel ToShopModel(UserDataDto dataDto)
    {
        var personalModel = ToPersonalModel(dataDto.personal);
        var warehouseModel = ToWarehouseModel(dataDto.warehouse);
        var designModel = ToDesignModel(dataDto.design);
        var shopObjects = ToObjectsModel(dataDto.objects);
        var unwashes = ToUnwashesModel(dataDto.unwashes);

        return new ShopModel(
            designModel,
            shopObjects,
            unwashes,
            personalModel,
            warehouseModel);
    }

    private Dictionary<Vector2Int, int> ToUnwashesModel(string[] unwashes)
    {
        var result = new Dictionary<Vector2Int, int>();
        if (unwashes != null)
        {
            ParseCoordsArrayAndFill(result, unwashes, (c, s) => int.Parse(s));
        }

        return result;
    }

    private ShopPersonalModel ToPersonalModel(string[] personal)
    {
        var result = new ShopPersonalModel();
        var personalConfig = GameConfigManager.Instance.PersonalConfig;
        if (personal != null)
        {
            foreach (var personalItem in personal)
            {
                var splitted = personalItem.Split('|');
                var config = personalConfig.GetPersonalConfigByStringId(splitted[0]);
                var endWorkTime = int.Parse(splitted[1]);
                result.SetPersonalWorkingTime(config, endWorkTime);
            }
        }

        return result;
    }

    private UserProgressModel ToProgressModel(ShopProgressDto progress)
    {
        var levelsConfig = GameConfigManager.Instance.LevelsConfig;
        return new UserProgressModel(levelsConfig, progress.cash, progress.gold, progress.exp);
    }

    private ShopWarehouseModel ToWarehouseModel(ShopWarehouseDto warehouseDto)
    {
        var result = new ShopWarehouseModel(warehouseDto.volume, warehouseDto.size);

        var products = warehouseDto.slots.Select(ToProductModel).ToArray();
        for (var i = 0; i < warehouseDto.size; i++)
        {
            if (i < products.Length)
            {
                result.Slots[i].SetProduct(products[i]);
            }
        }

        return result;
    }

    private Dictionary<Vector2Int, ShopObjectModelBase> ToObjectsModel(string[] objects)
    {
        var result = new Dictionary<Vector2Int, ShopObjectModelBase>();
        ParseCoordsArrayAndFill(result, objects, (c, s) => CreateShopObject(c, s.Split('|')));
        return result;
    }

    private ShoDesignModel ToDesignModel(ShopDesignDto designDto)
    {
        var parsedFloors = ParseFloors(designDto);
        var parsedWalls = ParseWalls(designDto);
        var parsedWindows = ParseCoordsArray(designDto.windows, (c, s) => int.Parse(s));
        var parsedDoors = ParseCoordsArray(designDto.doors, (c, s) => int.Parse(s));

        return new ShoDesignModel(
            designDto.size_x,
            designDto.size_y,
            parsedFloors,
            parsedWalls,
            parsedWindows,
            parsedDoors);
    }

    private Dictionary<Vector2Int, int> ParseWalls(ShopDesignDto designDto)
    {
        var result = new Dictionary<Vector2Int, int>();

        var std = GetStdId(designDto.walls);
        if (std > 0)
        {
            for (var x = 0; x < designDto.size_x; x++)
            {
                result[new Vector2Int(x, -1)] = std;
            }

            for (var y = 0; y < designDto.size_y; y++)
            {
                result[new Vector2Int(-1, y)] = std;
            }
        }

        ParseCoordsArrayAndFill(result, designDto.walls, (c, s) => int.Parse(s));

        return result;
    }

    private Dictionary<Vector2Int, int> ParseFloors(ShopDesignDto designDto)
    {
        var result = new Dictionary<Vector2Int, int>();

        var std = GetStdId(designDto.floors);
        if (std > 0)
        {
            for (var x = 0; x < designDto.size_x; x++)
            {
                for (var y = 0; y < designDto.size_y; y++)
                {
                    result[new Vector2Int(x, y)] = std;
                }
            }
        }

        ParseCoordsArrayAndFill(result, designDto.floors, (c, s) => int.Parse(s));

        return result;
    }

    public int GetStdId(string[] inputArray)
    {
        var result = -1;
        foreach (var item in inputArray)
        {
            if (item.IndexOf(Constants.StandardId) >= 0)
            {
                result = int.Parse(item.Split('|')[1]);
                break;
            }
        }

        return result;
    }

    private void ParseCoordsArrayAndFill<T>(Dictionary<Vector2Int, T> result, string[] inputArray, Func<Vector2Int, string, T> parseFunc)
    {
        var nonStdWalls = ParseCoordsArray(inputArray, parseFunc);
        foreach (var nonStdWall in nonStdWalls)
        {
            result[nonStdWall.Key] = nonStdWall.Value;
        }
    }

    private Dictionary<Vector2Int, T> ParseCoordsArray<T>(string[] array, Func<Vector2Int, string, T> parseFunc)
    {
        var result = new Dictionary<Vector2Int, T>();
        var delimiter = '|';
        var delimiterArr = new[] { delimiter };
        foreach (var item in array)
        {
            if (item.IndexOf(Constants.StandardId) >= 0) continue;
            var splitted = item.Split(delimiterArr, 2);
            var coords = ParseCoords(splitted[0]);
            var value = parseFunc(coords, splitted[1]);
            result[coords] = value;
        }
        return result;
    }

    private ShopWarehouseModel ToWarehouseModelOld(string warehouseStr)
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

    private ProductModel ToProductModel(string productStr)
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

    private ProductModel[] ToProducts(string objectParamsShort)
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

    private UserProgressModel ToProgressModelOld(GetDataOldResponseDataDto data)
    {
        var levelsConfig = GameConfigManager.Instance.LevelsConfig;
        return new UserProgressModel(
            levelsConfig,
            int.Parse(data.gold),
            int.Parse(data.exp),
            int.Parse(data.level));
    }

    private Dictionary<Vector2Int, ShopObjectModelBase> ToObjectsModelOld(string objects)
    {
        var result = new Dictionary<Vector2Int, ShopObjectModelBase>();
        var objectsConvertedDto = JsonConvert.DeserializeObject<Dictionary<string, string>>(objects);

        foreach (var kvp in objectsConvertedDto)
        {
            var coords = ParseCoords(kvp.Key);
            var paramsStr = kvp.Value.Split('|');
            result[coords] = CreateShopObjectOld(coords, paramsStr);
        }

        return result;
    }

    private ShopObjectModelBase CreateShopObjectOld(Vector2Int coords, string[] paramsStr)
    {
        var splitted1StParam = paramsStr[0].Split('_');
        var typeId = splitted1StParam[0];
        var numericId = int.Parse(splitted1StParam[1]);
        var angle = int.Parse(paramsStr[1]);
        var side = SideHelper.GetSideFromAngle(angle);
        var objectParamsShort = paramsStr[2];

        ShopObjectModelBase result = typeId switch
        {
            Constants.ShelfTypeStr => new ShopObjectModelFactory().CreateShelf(numericId, coords, side, ToProducts(objectParamsShort)),
            Constants.CashDeskTypeStr => new ShopObjectModelFactory().CreateCashDesk(numericId, coords, side),
            _ => throw new System.ArgumentOutOfRangeException(typeId, $"typeId {typeId} is not supported"),
        };
        return result;
    }

    private ShopObjectModelBase CreateShopObject(Vector2Int coords, string[] paramsStr)
    {
        var splitted1StParam = paramsStr[0].Split('_');
        var typeId = splitted1StParam[0];
        var numericId = int.Parse(splitted1StParam[1]);
        var side = int.Parse(paramsStr[1]);
        var objectParamsShort = paramsStr[2];

        ShopObjectModelBase result = typeId switch
        {
            Constants.ShelfTypeStr => new ShopObjectModelFactory().CreateShelf(numericId, coords, side, ToProducts(objectParamsShort)),
            Constants.CashDeskTypeStr => new ShopObjectModelFactory().CreateCashDesk(numericId, coords, side),
            _ => throw new System.ArgumentOutOfRangeException(typeId, $"typeId {typeId} is not supported"),
        };
        return result;
    }

    private ShoDesignModel ToDesignModelOld(string design)
    {
        var designConvertedDto = JsonConvert.DeserializeObject<DesignConvertedDto>(design);

        var sizeX = designConvertedDto.size_x;
        var sizeY = designConvertedDto.size_y;

        var convertedFloors = ConvertFloors(sizeX, sizeY, designConvertedDto.floors);
        var convertedWalls = ConvertWalls(sizeX, sizeY, designConvertedDto.walls);
        var convertedDoors = ConvertDoors(designConvertedDto.doors);
        var convertedWindows = ConvertWindows(designConvertedDto.windows);

        return new ShoDesignModel(sizeX, sizeY, convertedFloors, convertedWalls, convertedWindows, convertedDoors);
    }

    private Dictionary<Vector2Int, int> ConvertDoors(Dictionary<string, string> rawDoors)
    {
        var result = new Dictionary<Vector2Int, int>();
        foreach (var kvp in rawDoors)
        {
            var coords = ParseCoords(kvp.Key);
            result[coords] = int.Parse(kvp.Value);
        }

        return result;
    }

    private Dictionary<Vector2Int, int> ConvertWindows(Dictionary<string, string> rawWindows)
    {
        var result = new Dictionary<Vector2Int, int>();
        foreach (var kvp in rawWindows)
        {
            var coords = ParseCoords(kvp.Key);
            result[coords] = ParseIntItem("wnd_", kvp.Value);
        }

        return result;
    }

    private Dictionary<Vector2Int, int> ConvertWalls(int sizeX, int sizeY, Dictionary<string, string> rawWalls)
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

    private Dictionary<Vector2Int, int> ConvertFloors(int sizeX, int sizeY, Dictionary<string, string> rawData)
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

    private Vector2Int ParseCoords(string coordsStr)
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

    private int ParseIntItem(string prefix, string rawItemData)
    {
        return ParseIntItem(prefix, rawItemData, out var _);
    }

    private int ParseIntItem(string prefix, string rawItemData, out string[] splitResult)
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
