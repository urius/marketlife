using System.Collections.Generic;
using System.Linq;

public struct CalculateForReportCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerOfflineReportHolder = PlayerOfflineReportHolder.Instance;

        var calculationResult = playerModel.CalculateOfflineToTime(gameStateModel.ServerTime);

        for (var i = 0; i < calculationResult.UnwashesAddedAmount; i++)
        {
            playerModel.ShopModel.AddRandomUnwash();
        }

        var report = new UserOfflineReportModel(
            calculationResult.CalculationHours,
            calculationResult.SoldFromShelfs,
            calculationResult.SoldFromWarehouse,
            calculationResult.UnwashesCleanedAmount,
            CreateGuestOfflineActionModels(playerModel.ExternalActionsModel));
        playerOfflineReportHolder.SetReport(report);

        if (report.IsEmpty == false)
        {
            gameStateModel.ShowPopup(new OfflineReportPopupViewModel(report));
        }
        else
        {
            gameStateModel.SetGameState(GameStateName.PlayerShopSimulation);
        }
    }

    private IEnumerable<GuestOfflineReportActionModel> CreateGuestOfflineActionModels(ExternalActionsModel externalActionsModel)
    {
        var result = new List<GuestOfflineReportActionModel>();

        var groupedByUserId = externalActionsModel.Actions.GroupBy(m => m.PerformerId);
        foreach (var group in groupedByUserId)
        {
            var guestOfflineActionsModel = GetGuestActionsModel(group.Key, group);
            result.Add(guestOfflineActionsModel);
        }

        return result;
    }

    private GuestOfflineReportActionModel GetGuestActionsModel(string uid, IGrouping<string, ExternalActionModelBase> group)
    {
        var addedProductsDict = new Dictionary<ProductConfig, int>();
        var takenProductsDict = new Dictionary<ProductConfig, int>();
        var addedUnwashesCount = 0;
        foreach (var action in group)
        {
            switch (action.ActionId)
            {
                case FriendShopActionId.AddProduct:
                    var addProductAction = action as ExternalActionAddProduct;
                    if (addedProductsDict.ContainsKey(addProductAction.ProductConfig) == false)
                    {
                        addedProductsDict[addProductAction.ProductConfig] = 0;
                    }
                    addedProductsDict[addProductAction.ProductConfig] += addProductAction.Amount;
                    break;
                case FriendShopActionId.TakeProduct:
                    var takeProductAction = action as ExternalActionTakeProduct;
                    if (takenProductsDict.ContainsKey(takeProductAction.ProductConfig) == false)
                    {
                        takenProductsDict[takeProductAction.ProductConfig] = 0;
                    }
                    takenProductsDict[takeProductAction.ProductConfig] += takeProductAction.Amount;
                    break;
                case FriendShopActionId.AddUnwash:
                    addedUnwashesCount++;
                    break;
            }
        }

        var takenProducts = takenProductsDict.Select(kvp => new ProductModel(kvp.Key, kvp.Value)).ToArray();
        var addedProducts = addedProductsDict.Select(kvp => new ProductModel(kvp.Key, kvp.Value)).ToArray();

        return new GuestOfflineReportActionModel(uid, takenProducts, addedProducts, addedUnwashesCount);
    }
}
