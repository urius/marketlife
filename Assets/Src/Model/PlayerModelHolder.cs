using System;

public class PlayerModelHolder
{
    public static PlayerModelHolder Instance => _instance.Value;
    private static Lazy<PlayerModelHolder> _instance = new Lazy<PlayerModelHolder>();

    public event Action UidIsSet = delegate { };

    public string Uid { get; private set; }
    public UserModel UserModel { get; private set; }
    public ShopModel ShopModel => UserModel.ShopModel;

    public void SetUid(string uid)
    {
        Uid = uid;
        UidIsSet();
    }

    public void SetUserModel(UserModel userModel)
    {
        UserModel = userModel;
    }
}
