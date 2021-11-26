public struct HandleFriendShopActionClickCommand
{
    public void Execute(FriendShopActionId actionId, bool isBuyClicked)
    {
        var gameStateModel = GameStateModel.Instance;
        var viewingUserModel = gameStateModel.ViewingUserModel;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var friendActionsDataModel = playerModel.FriendsActionsDataModels.GetFriendShopActionsModel(viewingUserModel.Uid);
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var analyticsManager = AnalyticsManager.Instance;

        if (isBuyClicked == false)
        {
            switch (actionId)
            {
                case FriendShopActionId.AddProduct:
                    gameStateModel.ShowPopup(new WarehousePopupViewModel());
                    break;
                case FriendShopActionId.TakeProduct:
                    gameStateModel.SetTakeProductAction();
                    break;
                case FriendShopActionId.AddUnwash:
                    gameStateModel.SetAddUnwashAction();
                    break;
            }

            analyticsManager.SendCustom(AnalyticsManager.EventNameFriendActionClick, ("action", actionId.ToString()));
        }
        else
        {
            var priceGoldAmount = mainConfig.GetActionResetCooldownPriceGold(actionId);
            if (priceGoldAmount > 0)
            {
                var price = new Price(priceGoldAmount, isGold: true);
                var success = playerModel.TrySpendMoney(price);
                if (success)
                {
                    friendActionsDataModel.ActionsById[actionId].SetEndCooldownTime(gameStateModel.ServerTime - 1);
                }
                else
                {
                    new NotEnoughtMoneySequenceCommand().Execute(price.IsGold);
                }

                analyticsManager.SendCustom(AnalyticsManager.EventNameFriendActionBuyRecharge, ("action", actionId.ToString()), ("success", success));
            }
        }
    }
}
