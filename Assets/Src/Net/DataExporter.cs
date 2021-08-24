using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataExporter
{
    public static DataExporter Instance => _instance.Value;
    private static Lazy<DataExporter> _instance = new Lazy<DataExporter>();

    public ShopProgressDto ExportProgress(UserProgressModel progressModel)
    {
        return new ShopProgressDto(progressModel.Cash + progressModel.DelayedCash, progressModel.Gold, progressModel.ExpAmount, progressModel.Level);
    }

    public string[] ExportPersonal(ShopModel shopModel)
    {
        var result = new List<string>();

        var gameStateModel = GameStateModel.Instance;

        foreach (var personalItem in shopModel.PersonalModel.PersonalData)
        {
            if (personalItem.Value > gameStateModel.ServerTime)
            {
                var config = personalItem.Key;
                result.Add($"{config.TypeIdStr}_{config.NumericId}|{personalItem.Value}");
            }
        }

        return result.ToArray();
    }

    public ShopWarehouseDto ExportWarehouse(ShopModel shopModel)
    {
        var warehouseModel = shopModel.WarehouseModel;
        var slotsToExport = new string[warehouseModel.Size];

        for (var i = 0; i < slotsToExport.Length; i++)
        {
            slotsToExport[i] = ExportSlot(warehouseModel.Slots[i]);
        }

        return new ShopWarehouseDto(warehouseModel.Volume, slotsToExport);
    }

    public ShopDesignDto ExportDesign(ShopModel shopModel)
    {
        var shopDesignModel = shopModel.ShopDesign;

        var floorsExport = ExportCoordsDictionary(shopDesignModel.Floors, i => i.ToString(), useStd: true);
        var wallsExport = ExportOneCoordDictionary(shopDesignModel.Walls, i => i.ToString(), useStd: true);
        var windowsExport = ExportOneCoordDictionary(shopDesignModel.Windows, i => i.ToString());
        var doorsExport = ExportOneCoordDictionary(shopDesignModel.Doors, i => i.ToString());

        return new ShopDesignDto(shopDesignModel.SizeX, shopDesignModel.SizeY, floorsExport, wallsExport, windowsExport, doorsExport);
    }

    public string[] ExportShopObjects(ShopModel shopModel)
    {
        return ExportCoordsDictionary(shopModel.ShopObjects, ExportShopObject);
    }

    public string[] ExportUnwashes(ShopModel shopModel)
    {
        return ExportCoordsDictionary(shopModel.Unwashes, i => i.ToString());
    }

    public int[] ExportTutorialSteps(UserModel userModel)
    {
        return userModel.TutorialSteps.ToArray();
    }

    public string[] ExportAvailableActionsData(UserModel userModel)
    {
        var result = new List<string>(AvailableFriendShopActionsDataModel.SupportedActionsCount);
        foreach (var actionData in userModel.ActionsDataModel.ActionsData)
        {
            result.Add($"{actionData.ActionIdInt}|{actionData.RestAmount}|{actionData.EndCooldownTimestamp}");
        }

        return result.ToArray();
    }

    private string ExportShopObject(ShopObjectModelBase shopObjectModel)
    {
        string objectIdStr = null;
        string objectParametersStr = string.Empty;
        switch (shopObjectModel.Type)
        {
            case ShopObjectType.CashDesk:
                var cashDeskModel = shopObjectModel as CashDeskModel;
                objectIdStr = $"{Constants.CashDeskTypeStr}_{cashDeskModel.NumericId}";
                break;
            case ShopObjectType.Shelf:
                var shelfModel = shopObjectModel as ShelfModel;
                objectIdStr = $"{Constants.ShelfTypeStr}_{shelfModel.NumericId}";
                objectParametersStr = ExportShelfParameters(shelfModel);
                break;
        }

        return $"{objectIdStr}|{shopObjectModel.Side}|{objectParametersStr}";
    }

    private string ExportShelfParameters(ShelfModel shelfModel)
    {
        var result = string.Empty;
        var slots = shelfModel.Slots;
        for (var i = 0; i < slots.Length; i++)
        {
            var exportedSlot = ExportSlot(slots[i]);
            if (exportedSlot != null)
            {
                result += exportedSlot;
            }
            if (i < slots.Length - 1)
            {
                result += Constants.ShelfProductsDelimiter;
            }
        }

        return result;
    }

    private string[] ExportCoordsDictionary<TIn>(Dictionary<Vector2Int, TIn> inputDictionary, Func<TIn, string> convertValuesFunc, bool useStd = false)
    {
        var result = new List<string>(inputDictionary.Count);

        var stdItemId = default(TIn);
        if (useStd)
        {
            stdItemId = GetStandardItem(inputDictionary);
            result.Add($"{Constants.StandardId}|{convertValuesFunc(stdItemId)}");
        }

        foreach (var kvp in inputDictionary)
        {
            if (!useStd || (useStd && !kvp.Value.Equals(stdItemId)))
            {
                result.Add($"x{kvp.Key.x}y{kvp.Key.y}|{convertValuesFunc(kvp.Value)}");
            }
        }

        return result.ToArray();
    }

    private TIn GetStandardItem<TIn>(Dictionary<Vector2Int, TIn> inputDictionary)
    {
        var counterDictionary = new Dictionary<TIn, int>();
        var maxItemId = inputDictionary.Values.First();
        foreach (var kvp in inputDictionary)
        {
            var itemId = kvp.Value;
            if (counterDictionary.ContainsKey(kvp.Value))
            {
                counterDictionary[itemId]++;
            }
            else
            {
                counterDictionary[itemId] = 1;
            }

            if (!itemId.Equals(maxItemId) && counterDictionary[itemId] > counterDictionary[maxItemId])
            {
                maxItemId = itemId;
            }
        }
        return maxItemId;
    }

    private string[] ExportOneCoordDictionary<TIn>(Dictionary<Vector2Int, TIn> inputDictionary, Func<TIn, string> convertValuesFunc, bool useStd = false)
    {
        var result = new List<string>();

        var stdItemId = default(TIn);
        if (useStd)
        {
            stdItemId = GetStandardItem(inputDictionary);
            result.Add($"{Constants.StandardId}|{convertValuesFunc(stdItemId)}");
        }

        foreach (var kvp in inputDictionary)
        {
            if (!useStd || (useStd && !kvp.Value.Equals(stdItemId)))
            {
                if (kvp.Key.x < 0)
                {
                    result.Add($"y{kvp.Key.y}|{convertValuesFunc(kvp.Value)}");
                }
                else
                {
                    result.Add($"x{kvp.Key.x}|{convertValuesFunc(kvp.Value)}");
                }
            }
        }

        return result.ToArray();
    }

    private string ExportSlot(ProductSlotModel productSlotModel)
    {
        var gameStateModel = GameStateModel.Instance;

        if (productSlotModel.HasProduct)
        {
            var productModel = productSlotModel.Product;
            var productBaseString = $"p{productModel.Config.NumericId}x{productModel.Amount}";
            return productModel.DeliverTime > gameStateModel.ServerTime ?
                $"{productBaseString}d{productModel.DeliverTime}"
                : productBaseString;
        }

        return null;
    }

}
