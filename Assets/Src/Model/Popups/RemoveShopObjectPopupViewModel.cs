public class RemoveShopObjectPopupViewModel : PopupViewModelBase, ISellPriceModel
{
    public RemoveShopObjectPopupViewModel(ShopObjectModelBase shopObjectModel)
    {
        ShopObjectModel = shopObjectModel;
    }

    public ShopObjectModelBase ShopObjectModel { get; private set; }
    public int SellPrice => ShopObjectModel.SellPrice;

    public override PopupType PopupType => PopupType.RemoveShopObjectPopup;
}
