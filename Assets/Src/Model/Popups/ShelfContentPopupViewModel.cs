using Src.Model.ShopObjects;

namespace Src.Model.Popups
{
    public class ShelfContentPopupViewModel : PopupViewModelBase
    {
        public readonly ShelfModel ShelfModel;

        public ShelfContentPopupViewModel(ShelfModel shelfModel)
        {
            ShelfModel = shelfModel;
        }

        public override PopupType PopupType => PopupType.ShelfContent;

    }
}
