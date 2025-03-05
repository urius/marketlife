
namespace Src.Model.Popups
{
    public class BillboardPopupViewModel : PopupViewModelBase
    {
        public readonly string InitialText;

        public BillboardPopupViewModel(ShopBillboardModel model)
        {
            InitialText = model.Text;
        }

        public override PopupType PopupType => PopupType.Billboard;
    }
}
