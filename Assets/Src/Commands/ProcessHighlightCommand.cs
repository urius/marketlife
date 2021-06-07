using UnityEngine;

public struct ProcessHighlightCommand
{
    public void Execute(Vector2Int coords)
    {
        var gameStateModel = GameStateModel.Instance;

        if (gameStateModel.PlacingState != PlacingStateName.None && gameStateModel.PlacingState != PlacingStateName.PlacingProduct)
        {
            return;
        }

        var shopModel = gameStateModel.ViewingShopModel;
        if (gameStateModel.PlacingState == PlacingStateName.PlacingProduct)
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
            else if (shopModel.ShopDesign.IsHighlightableDecorationOn(coords))
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
