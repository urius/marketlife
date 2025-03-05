using System;
using System.Collections.Generic;
using Src.Managers;
using Src.Model;
using Src.Model.ShopObjects;

namespace Src.Commands
{
    public struct AutoPlaceCommand
    {
        public void Execute()
        {
            var gameStateModel = GameStateModel.Instance;
            var viewingUserModel = gameStateModel.ViewingUserModel;
            var viewingShopModel = gameStateModel.ViewingShopModel;
            var playerShopModel = PlayerModelHolder.Instance.UserModel.ShopModel;
            var audioManager = AudioManager.Instance;
            var analyticsManager = AnalyticsManager.Instance;
            var isSuccess = false;

            var warehouseSlot = playerShopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];

            var productsAddedCount = 0;
            var notEmptyShelfSlots = GetFilteredShelfSlots(viewingShopModel, FilterNotEmpty);
            foreach (var shelfSlotData in notEmptyShelfSlots)
            {
                if (shelfSlotData.ShelfModel.Slots[shelfSlotData.SlotIndex].Product.NumericId == warehouseSlot.Product.NumericId)
                {
                    productsAddedCount += new PutWarehouseProductOnShelfCommand().Execute(warehouseSlot.Index, viewingUserModel, shelfSlotData.ShelfModel, shelfSlotData.SlotIndex);
                    if (warehouseSlot.HasProduct == false) break;
                }
            }
            var emptyShelfSlots = GetFilteredShelfSlots(viewingShopModel, FilterEmpty);
            foreach (var shelfSlotData in emptyShelfSlots)
            {
                productsAddedCount += new PutWarehouseProductOnShelfCommand().Execute(warehouseSlot.Index, viewingUserModel, shelfSlotData.ShelfModel, shelfSlotData.SlotIndex);
                if (warehouseSlot.HasProduct == false) break;
            }

            if (productsAddedCount > 0)
            {
                audioManager.PlaySound(SoundNames.ProductPut);
                isSuccess = true;
            }

            gameStateModel.ResetActionState();

            analyticsManager.SendCustom(AnalyticsManager.EventAutoPlaceClick, ("is_success", isSuccess));
        }

        private IEnumerable<(Model.ShopObjects.ShelfModel ShelfModel, int SlotIndex)> GetFilteredShelfSlots(ShopModel shopModel, Func<ProductSlotModel, bool> filterFunc)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;

            foreach (var kvp in shopModel.ShopObjects)
            {
                if (kvp.Value.Type == ShopObjectType.Shelf)
                {
                    var shelfModel = kvp.Value as ShelfModel;
                    foreach (var shelfSlot in shelfModel.Slots)
                    {
                        if (filterFunc(shelfSlot) == true) yield return (shelfModel, shelfSlot.Index);
                    }
                }
            }
        }

        private bool FilterNotEmpty(ProductSlotModel slotModel)
        {
            return slotModel.HasProduct;
        }

        private bool FilterEmpty(ProductSlotModel slotModel)
        {
            return !slotModel.HasProduct;
        }
    }
}
