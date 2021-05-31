public class RemoveShopObjectPopupViewModel : PopupViewModelBase, IConfirmRemoveObjectPopupViewModel
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
