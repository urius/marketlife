using System;
using Cysharp.Threading.Tasks;

public class PlayerModelHolder
{
    public static PlayerModelHolder Instance => _instance.Value;
    private static Lazy<PlayerModelHolder> _instance = new Lazy<PlayerModelHolder>();

    public event Action UserModelIsSet = delegate { };

    private UniTaskCompletionSource _setUidTcs = new UniTaskCompletionSource();

    public string Uid { get; private set; }
    public SocialType SocialType { get; private set; }
    public bool IsBuyInBankAllowed { get; private set; }
    public string UserDataStr { get; private set; }
    public UserModel UserModel { get; private set; }
    public ShopModel ShopModel => UserModel.ShopModel;
    public UniTask SetUidTask => _setUidTcs.Task;

    public void SetSocialType(SocialType socialType)
    {
        SocialType = socialType;
    }

    public void SetBuyInBankAllowed(bool isAllowed)
    {
        IsBuyInBankAllowed = isAllowed;
    }

    public void SetUid(string uid)
    {
        Uid = uid;
        _setUidTcs.TrySetResult();
    }

    public void SetUserModel(UserModel userModel)
    {
        UserModel = userModel;
        UserModelIsSet();
    }

    public void SetUserDataRaw(string userDataStr)
    {
        UserDataStr = userDataStr;
    }
}

public enum SocialType
{
    Undefined,
    VK,
}