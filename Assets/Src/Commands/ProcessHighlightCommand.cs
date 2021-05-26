using UnityEngine;

public struct ProcessHighlightCommand
{
    public void Execute(Vector2Int coords)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
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
