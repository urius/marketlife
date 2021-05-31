using UnityEngine;

public class RemoveShopDecorationPopupViewModel : PopupViewModelBase, IConfirmRemoveObjectPopupViewModel
{
    private readonly Vector2Int _coords;
    private readonly int _sellPrice;

    public RemoveShopDecorationPopupViewModel(Vector2Int coords, int sellPrice)
    {
        _coords = coords;
        _sellPrice = sellPrice;
    }

    public int SellPrice => _sellPrice;
    public Vector2Int ObjectCoords => _coords;

    public override PopupType PopupType => PopupType.ConfirmRemoveObject;
}
