using UnityEngine;

public struct OrderProductCommand
{
    public void Execute(RectTransform transform, Vector2 startAnimationScreenPoint, ProductConfig productConfig)
    {
        var dispatcher = Dispatcher.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var shopModel = playerModel.ShopModel;
        var warehouseVolume = shopModel.WarehouseModel.Volume;

        var price = productConfig.GetPriceForVolume(warehouseVolume);
        if (playerModel.TrySpendMoney(price))
        {
            var productModel = new ProductModel(
                productConfig,
                productConfig.GetAmountInVolume(warehouseVolume),
                gameStateModel.ServerTime + productConfig.DeliverTimeSeconds);
            if (gameStateModel.ShowingPopupModel is OrderProductPopupViewModel orderProductPopupModel)
            {
                var warehouseSlotIndex = orderProductPopupModel.TargetWarehouseSlotIndex;
                dispatcher.UIRequestOrderProductAnimation(transform, startAnimationScreenPoint, warehouseSlotIndex, productModel);
                shopModel.WarehouseModel.Slots[warehouseSlotIndex].SetProduct(productModel);
                gameStateModel.RemoveCurrentPopupIfNeeded();
            }

            AnalyticsManager.Instance.SendCustom(AnalyticsManager.EventNameOrderProduct, ("product", productConfig.NumericId));
        }
        else
        {
            dispatcher.UIRequestBlinkMoney(price.IsGold);
        }
    }
}
