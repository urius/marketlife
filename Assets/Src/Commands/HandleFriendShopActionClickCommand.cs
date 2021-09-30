public struct HandleFriendShopActionClickCommand
{
    public void Execute(FriendShopActionId actionId, bool isBuyClicked)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerActionsDataModel = playerModel.ActionsDataModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var dispatcher = Dispatcher.Instance;
        var analyticsManager = AnalyticsManager.Instance;

        if (isBuyClicked == false)
        {
            switch (actionId)
            {
                case FriendShopActionId.Take:
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
            var price = new Price(mainConfig.ActionResetCooldownPrice, isGold: true);
            var success = playerModel.TrySpendMoney(price);
            if (success)
            {
                playerActionsDataModel.ActionsById[actionId].SetEndCooldownTime(gameStateModel.ServerTime - 1);
            }
            else
            {
                dispatcher.UIRequestBlinkMoney(price.IsGold);
            }

            analyticsManager.SendCustom(AnalyticsManager.EventNameFriendActionBuyRecharge, ("action", actionId.ToString()), ("success", success));
        }
    }
}
