using Src.Model;
using Src.Model.ShopObjects;

namespace Src.Commands
{
    public struct PutWarehouseProductOnShelfCommand
    {
        public int Execute(int warehouseSlotIndex, UserModel targetUserModel, ShelfModel shelfModel, int shelfSlotIndex)
        {
            var playerModelHolder = PlayerModelHolder.Instance;
            var playerModel = playerModelHolder.UserModel;
            var playerShopModel = playerModelHolder.ShopModel;
            var warehouseSlot = playerShopModel.WarehouseModel.Slots[warehouseSlotIndex];

            var placedProductsCount = 0;
            if (warehouseSlot.HasProduct)
            {
                var shelfSlot = shelfModel.Slots[shelfSlotIndex];
                var isPlayerShop = playerModel == targetUserModel;
                var placingProductConfig = warehouseSlot.Product.Config;

                placedProductsCount = new PutProductFromSlotToSlotCommand().Execute(warehouseSlot, shelfSlot);

                if (isPlayerShop == false && placedProductsCount > 0)
                {
                    targetUserModel.ExternalActionsModel.AddAction(new ExternalActionAddProduct(playerModel.Uid, shelfModel.Coords, shelfSlotIndex, placingProductConfig, placedProductsCount));
                }
            }

            return placedProductsCount;
        }
    }
}
