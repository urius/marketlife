using System;
using Cysharp.Threading.Tasks;

public class PlayerModelHolder
{
    public static PlayerModelHolder Instance => _instance.Value;
    private static Lazy<PlayerModelHolder> _instance = new Lazy<PlayerModelHolder>();

    private UniTaskCompletionSource _setUidTcs = new UniTaskCompletionSource();

    public string Uid { get; private set; }
    public PlatformType PlatformType { get; private set; }
    public UserModel UserModel { get; private set; }
    public ShopModel ShopModel => UserModel.ShopModel;
    public UniTask SetUidTask => _setUidTcs.Task;

    public void SetPlatformType(PlatformType platformType)
    {
        PlatformType = platformType;
    }

    public void SetUid(string uid)
    {
        Uid = uid;
        _setUidTcs.TrySetResult();
    }

    public void SetUserModel(UserModel userModel)
    {
        UserModel = userModel;
    }
}

public enum PlatformType
{
    Undefined,
    VK,
}