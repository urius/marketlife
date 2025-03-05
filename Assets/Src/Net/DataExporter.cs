using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Src.Common;
using Src.Common_Utils;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Missions;
using Src.Model.ShopObjects;
using UnityEngine;

namespace Src.Net
{
    public class DataExporter
    {
        public static DataExporter Instance => _instance.Value;
        private static Lazy<DataExporter> _instance = new Lazy<DataExporter>();

        public BonusStateDto ExportBonus(UserBonusState bonusState)
        {
            return new BonusStateDto
            {
                timestamp = bonusState.LastBonusTakeTimestamp,
                rank = bonusState.LastTakenBonusRank,
            };
        }

        public string[] ExportDailyMissions(DailyMissionsModel dailyMissionsModel)
        {
            var result = new string[dailyMissionsModel.MissionsList.Count];
            var i = 0;
            foreach (var mission in dailyMissionsModel.MissionsList)
            {
                var isRewardTakenInt = mission.IsRewardTaken ? 1 : 0;
                var localResultStr = $"{mission.Key},{mission.StartValue},{mission.TargetValue},{mission.Value},{mission.Reward.SerializeToString()},{isRewardTakenInt}";
                switch (mission.Key)
                {
                    case MissionKeys.SellProduct:
                        var sellProductMission = mission as DailyMissionSellProductModel;
                        var productStr = ExportProduct(sellProductMission.ProductConfig, mission.TargetValue);
                        localResultStr = $"{localResultStr},{productStr}";
                        break;
                    case MissionKeys.AddShelfs:
                        var addShelfsMission = mission as DailyMissionAddShelfsModel;
                        var shelfStr = $"s{addShelfsMission.ShelfNumericId}";
                        localResultStr = $"{localResultStr},{shelfStr}";
                        break;
                }
                result[i] = localResultStr;
                i++;
            }
            return result;
        }

        public UserBillboardStateDto ExportBillboardState(ShopBillboardModel billboardModel)
        {
            return new UserBillboardStateDto()
            {
                is_available = billboardModel.IsAvailable,
                text_64 = Base64Helper.Base64Encode(billboardModel.Text),
            };
        }

        public UserGameSettingsDto ExportSettings(UserSettingsModel settingsModel)
        {
            return new UserGameSettingsDto()
            {
                mute_audio = settingsModel.IsAudioMuted,
                mute_music = settingsModel.IsMusicMuted,
            };
        }

        public ShopProgressDto ExportProgress(UserProgressModel progressModel)
        {
            return new ShopProgressDto(progressModel.Cash + progressModel.DelayedCash, progressModel.Gold, progressModel.ExpAmount, progressModel.Level);
        }

        public string[] ExportExternalActions(ExternalActionsModel externalActionsModel)
        {
            var resultDictionary = new Dictionary<string, Queue<ExternalActionModelBase>>();
            foreach (var actionModel in externalActionsModel.Actions)
            {
                if (resultDictionary.ContainsKey(actionModel.PerformerId) == false)
                {
                    resultDictionary[actionModel.PerformerId] = new Queue<ExternalActionModelBase>();
                }

                resultDictionary[actionModel.PerformerId].Enqueue(actionModel);
            }

            var result = new List<string>(resultDictionary.Count);
            foreach (var kvp in resultDictionary)
            {
                var sb = new StringBuilder();
                sb.Append($"{kvp.Key}:{ExportAction(kvp.Value.Dequeue())}");
                while (kvp.Value.Count > 0)
                {
                    sb.Append($";{ExportAction(kvp.Value.Dequeue())}");
                }
                result.Add(sb.ToString());
            }

            return result.ToArray();
        }

        public LeaderboardSaveItemDto ExportLeaderboardPlayerData(UserProgressModel playerProgress, int friendsCount)
        {
            var result = new LeaderboardSaveItemDto
            {
                cash = playerProgress.Cash,
                gold = playerProgress.Gold,
                exp = playerProgress.ExpAmount,
                friends = friendsCount,
            };

            return result;
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
            var friendsActionsDataModels = userModel.FriendsActionsDataModels;

            var result = new List<string>(friendsActionsDataModels.FriendShopActionsModelByUid.Count);
            foreach (var kvp in friendsActionsDataModels.FriendShopActionsModelByUid)
            {
                result.Add($"{kvp.Key}:{ExportFriendActionsDataStr(kvp.Value)}");
            }

            return result.ToArray();
        }

        private string ExportFriendActionsDataStr(FriendShopActionsModel actionsModel)
        {
            var resultArr = actionsModel.ActionsData
                .Select(a => $"{(int)a.ActionId}|{a.RestAmount}|{a.EndCooldownTimestamp}")
                .ToArray();
            return string.Join(",", resultArr);
        }

        private string ExportAction(ExternalActionModelBase action)
        {
            switch (action.ActionId)
            {
                case FriendShopActionId.TakeProduct:
                    var takeAction = action as ExternalActionTakeProduct;
                    return $"{(int)takeAction.ActionId}|{ExportCoords(takeAction.Coords)}|{ExportProduct(takeAction.ProductConfig, takeAction.Amount)}";
                case FriendShopActionId.AddProduct:
                    var addProductAction = action as ExternalActionAddProduct;
                    return $"{(int)addProductAction.ActionId}|{ExportCoords(addProductAction.Coords)}|{addProductAction.ShelfSlotIndex}|{ExportProduct(addProductAction.ProductConfig, addProductAction.Amount)}";
                case FriendShopActionId.AddUnwash:
                    var addUnwashAction = action as ExternalActionAddUnwash;
                    return $"{(int)addUnwashAction.ActionId}|{ExportCoords(addUnwashAction.Coords)}";
            }

            return $"?{(int)action.ActionId}";
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
                    objectParametersStr = $"{cashDeskModel.HairId},{cashDeskModel.GlassesId},{cashDeskModel.DressId}";
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
                    result.Add($"{ExportCoords(kvp.Key)}|{convertValuesFunc(kvp.Value)}");
                }
            }

            return result.ToArray();
        }

        private string ExportCoords(Vector2Int coords)
        {
            return $"x{coords.x}y{coords.y}";
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
            if (productSlotModel.HasProduct)
            {
                var productModel = productSlotModel.Product;
                return ExportProduct(productModel.Config, productModel.Amount, productModel.DeliverTime);
            }

            return null;
        }

        private string ExportProduct(ProductConfig productConfig, int amount, int deliverTime = 0)
        {
            var gameStateModel = GameStateModel.Instance;
            var productBaseString = $"p{productConfig.NumericId}x{amount}";
            return deliverTime > gameStateModel.ServerTime ?
                $"{productBaseString}d{deliverTime}"
                : productBaseString;
        }
    }
}
