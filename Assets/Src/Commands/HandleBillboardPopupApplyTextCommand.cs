using Src.Managers;
using Src.Model;

namespace Src.Commands
{
    public struct HandleBillboardPopupApplyTextCommand
    {
        public void Execute(string text)
        {
            var playerModelHolder = PlayerModelHolder.Instance;
            var billboardModel = playerModelHolder.UserModel.ShopModel.BillboardModel;
            var config = GameConfigManager.Instance.MainConfig;

            if (text.Length > config.MaxBillboardTextLength)
            {
                text = text.Substring(0, config.MaxBillboardTextLength);
            }
            billboardModel.SetText(text);
        }
    }
}
