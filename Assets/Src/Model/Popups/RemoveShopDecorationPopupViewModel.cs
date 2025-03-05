using UnityEngine;

namespace Src.Model.Popups
{
    public class RemoveShopDecorationPopupViewModel : ConfirmPopupViewModel, IConfirmRemoveWithRefundPopupViewModel
    {
        private readonly Vector2Int _coords;
        private readonly int _sellPrice;

        public RemoveShopDecorationPopupViewModel(string titleText, string messageText, Vector2Int coords, int sellPrice)
            : base(titleText, messageText)
        {
            _coords = coords;
            _sellPrice = sellPrice;
        }

        public int SellPrice => _sellPrice;
        public Vector2Int ObjectCoords => _coords;
    }
}
