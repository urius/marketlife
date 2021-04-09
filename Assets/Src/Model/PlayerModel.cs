using System;

public class PlayerModel
{
    public static PlayerModel Instance => GetOrCreateInstance();
    private static PlayerModel _instance;

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

    private static PlayerModel GetOrCreateInstance()
    {
        if (_instance == null)
        {
            _instance = new PlayerModel();
        }

        return _instance;
    }
}
