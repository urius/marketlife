using System;

public class PlayerModel
{
    public static PlayerModel Instance => _instance.Value;
    private static Lazy<PlayerModel> _instance = new Lazy<PlayerModel>();

    public string Uid { get; private set; }
    public int Cash { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }
    public int Level { get; private set; }
    public ShopModel ShopModel { get; private set; }

    public void SetUid(string uid)
    {
        Uid = uid;
    }

    public void SetValues(int cash, int gold, int exp, int level)
    {
        Cash = cash;
        Gold = gold;
        Exp = exp;
        Level = level;
    }

    public void SetShopModel(ShopModel shopModel)
    {
        ShopModel = shopModel;
    }
}
