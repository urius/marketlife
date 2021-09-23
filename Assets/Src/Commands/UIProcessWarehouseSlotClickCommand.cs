using System.Linq;

public struct UIProcessWarehouseSlotClickCommand
{
    public void Execute(int slotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;
        var warehouseModel = playerModelHolder.ShopModel.WarehouseModel;

        var slotModel = warehouseModel.Slots[slotIndex];
        if (slotModel.Product != null)
        {
            if (slotModel.Product.DeliverTime <= gameStateModel.ServerTime)
            {
                gameStateModel.SetPlacingProductSlotIndex(slotIndex);
            }
        }
        else
        {
            var popupModel = GetPopupModel(slotIndex);
            gameStateModel.CachePopup(popupModel);
            gameStateModel.ShowPopup(popupModel);
        }
    }

    private OrderProductPopupViewModel GetPopupModel(int slotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var productsConfig = GameConfigManager.Instance.ProductsConfig;

        var result = (gameStateModel.GetPopupFromCache(PopupType.OrderProduct) ?? new OrderProductPopupViewModel()) as OrderProductPopupViewModel;

        var productsByGroupId = productsConfig.GetProductConfigsForLevel(playerModel.ProgressModel.Level)
            .GroupBy(p => p.GroupId)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.UnlockLevel).ToArray());

        result.Setup(slotIndex, productsByGroupId.Keys.ToArray(), productsByGroupId);

        return result;
    }
}
