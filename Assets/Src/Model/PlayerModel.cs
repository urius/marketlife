using System;

public class PlayerModel
{
    public static PlayerModel Instance => _instance.Value;
    private static Lazy<PlayerModel> _instance = new Lazy<PlayerModel>();

    public string Uid { get; private set; }
    public ShopModel ShopModel { get; private set; }

    public void SetUid(string uid)
    {
        Uid = uid;
    }

    public void SetShopModel(ShopModel shopModel)
    {
        ShopModel = shopModel;
    }
}
