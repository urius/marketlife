using UnityEngine;

public struct ProcessHighlightCommand
{
    public void Execute(Vector2Int coords)
    {
        var gameStateModel = GameStateModel.Instance;
        var gameState = gameStateModel.GameState;

        if (gameStateModel.ActionState != ActionStateName.None
            && gameStateModel.ActionState != ActionStateName.PlacingProductPlayer
            && gameStateModel.ActionState != ActionStateName.PlacingProductFriend
            && gameStateModel.ActionState != ActionStateName.FriendShopTakeProduct
            && gameStateModel.ActionState != ActionStateName.FriendShopAddUnwash)
        {
            return;
        }

        var shopModel = gameStateModel.ViewingShopModel;
        if (gameStateModel.ActionState == ActionStateName.PlacingProductPlayer
            || gameStateModel.ActionState == ActionStateName.PlacingProductFriend)
        {
            if (shopModel.Grid.TryGetValue(coords, out var shopObjectData)
                             && shopObjectData.buildState > 0
                             && shopObjectData.reference.Type == ShopObjectType.Shelf)
            {
                gameStateModel.SetHighlightedShopObject(shopObjectData.reference);
            }
            else
            {
                gameStateModel.ResetHighlightedState();
            }
        }
        else
        {
            if (shopModel.Grid.TryGetValue(coords, out var shopObjectData)
                 && shopObjectData.buildState > 0)
            {
                gameStateModel.SetHighlightedShopObject(shopObjectData.reference);
            }
            else if (gameState != GameStateName.PlayerShopInterior && shopModel.Unwashes.TryGetValue(coords, out var unwashNumericId))
            {
                gameStateModel.SetHighlightedUnwashOn(coords);
            }
            else if (gameState != GameStateName.PlayerShopSimulation && shopModel.ShopDesign.IsHighlightableDecorationOn(coords))
            {
                gameStateModel.SetHighlightedDecorationOn(coords);
            }
            else
            {
                gameStateModel.ResetHighlightedState();
            }
        }
    }
}
