using System;

public class PlayerModelHolder
{
    public static PlayerModelHolder Instance => _instance.Value;
    private static Lazy<PlayerModelHolder> _instance = new Lazy<PlayerModelHolder>();

    public string Uid { get; private set; }
    public UserModel UserModel { get; private set; }
    public ShopModel ShopModel => UserModel.ShopModel;

    public void SetUid(string uid)
    {
        Uid = uid;
    }

    public void SetUserModel(UserModel userModel)
    {
        UserModel = userModel;
    }
}
