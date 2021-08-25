public struct HandleFriendShopActionClickCommand
{
    public void Execute(FriendShopActionId actionId, bool isBuyClicked)
    {
        var gameStateModel = GameStateModel.Instance;
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
        }
    }
}
