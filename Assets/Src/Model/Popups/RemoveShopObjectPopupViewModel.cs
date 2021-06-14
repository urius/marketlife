public class RemoveShopObjectPopupViewModel : PopupViewModelBase, IConfirmRemoveWithRefundPopupViewModel
{
    public RemoveShopObjectPopupViewModel(ShopObjectModelBase shopObjectModel, int sellPrice)
    {
        ShopObjectModel = shopObjectModel;
        SellPrice = sellPrice;
    }

    public ShopObjectModelBase ShopObjectModel { get; }
    public int SellPrice { get; }

    public override PopupType PopupType => PopupType.ConfirmRemoveObject;
}
