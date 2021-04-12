using System;

public class PlayerModel
{
    public static PlayerModel Instance => _instance.Value;
    private static Lazy<PlayerModel> _instance = new Lazy<PlayerModel>();

    public event Action<ShopModel> ViewingShopModelChanged = delegate { };

    public string Uid { get; private set; }
    public ShopModel ShopModel { get; private set; }

    public ShopModel ViewingShopModel { get; private set; }

    public void SetUid(string uid)
    {
        Uid = uid;
    }

    public void SetShopModel(ShopModel shopModel)
    {
        ShopModel = shopModel;
    }

    public void SetViewingShopModel(ShopModel shopModel)
    {
        ViewingShopModel = shopModel;

        ViewingShopModelChanged(ViewingShopModel);
    }
}
