using Src.Model.ShopObjects;

namespace Src.Model.Popups
{
    public class RemoveShopObjectPopupViewModel : ConfirmPopupViewModel, IConfirmRemoveWithRefundPopupViewModel
    {
        public RemoveShopObjectPopupViewModel(string titleText, string messageText, ShopObjectModelBase shopObjectModel, int sellPrice)
            : base(titleText, messageText)
        {
            ShopObjectModel = shopObjectModel;
            SellPrice = sellPrice;
        }

        public ShopObjectModelBase ShopObjectModel { get; }
        public int SellPrice { get; }
    }
}
