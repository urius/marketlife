public struct RotatePlacedObjectCommand
{
    public void Execute(ShopObjectModelBase shopObjectModel, int deltaSide)
    {
        var shopModel = GameStateModel.Instance.ViewingShopModel;
        if (shopModel.CanRotateShopObject(shopObjectModel, deltaSide))
        {
            shopModel.RotateShopObject(shopObjectModel, deltaSide);
        }
        else if (shopModel.CanRotateShopObject(shopObjectModel, 2 * deltaSide))
        {
            shopModel.RotateShopObject(shopObjectModel, 2 * deltaSide);
        }
        else
        {
            //TODO: "Can't rotate" Sound
        }
    }
}
