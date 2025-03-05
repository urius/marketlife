using System;
using Cysharp.Threading.Tasks;

namespace Src.Model
{
    public class PlayerModelHolder
    {
        public static PlayerModelHolder Instance => _instance.Value;
        private static Lazy<PlayerModelHolder> _instance = new Lazy<PlayerModelHolder>();

        public event Action UserModelIsSet = delegate { };

        private readonly UniTaskCompletionSource _setUidTcs = new UniTaskCompletionSource();
        private readonly UniTaskCompletionSource _setUserModelTcs = new UniTaskCompletionSource();

        public string Uid { get; private set; }
        public SocialType SocialType { get; private set; }
        public bool IsBuyInBankAllowed { get; private set; }
        public string UserDataStr { get; private set; }
        public UserModel UserModel { get; private set; }
        public ShopModel ShopModel => UserModel.ShopModel;
        public UniTask SetUidTask => _setUidTcs.Task;
        public UniTask SetUserModelTask => _setUserModelTcs.Task;

        public void SetBuyInBankAllowed(bool isAllowed)
        {
            IsBuyInBankAllowed = isAllowed;
        }

        public void SetInitialData(string userUid, SocialType socialType)
        {
            SocialType = socialType;
            Uid = userUid;
            _setUidTcs.TrySetResult();
        }

        public void SetUserModel(UserModel userModel)
        {
            UserModel = userModel;
        
            _setUserModelTcs.TrySetResult();
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
        YG,
    }
}