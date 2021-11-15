using UnityEngine;

public struct HandleConfirmPopupResultCommand
{
    public void Execute(bool result)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shopModel = gameStateModel.ViewingShopModel;
        var dispatcher = Dispatcher.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        var audioManader = AudioManager.Instance;

        if (result)
        {
            var popupModel = gameStateModel.ShowingPopupModel;
            Vector2Int coords = Vector2Int.zero;
            if (popupModel is RemoveShopObjectPopupViewModel removeShopObjectPopupViewModel)
            {
                var shopObjectModel = removeShopObjectPopupViewModel.ShopObjectModel;
                shopModel.RemoveShopObject(shopObjectModel);
                coords = shopObjectModel.Coords;
                audioManader.PlaySound(SoundNames.Remove);
            }
            else if (popupModel is RemoveShopDecorationPopupViewModel removeShopDecorationPopupViewModel)
            {
                coords = removeShopDecorationPopupViewModel.ObjectCoords;
                var removeResult = shopModel.TryRemoveDecoration(coords);
                if (removeResult == false)
                {
                    return;
                }
                else
                {
                    audioManader.PlaySound(SoundNames.Remove);
                }
            }
            else if (popupModel is RemoveProductPopupViewModel removeProductPopupViewModel)
            {
                shopModel.WarehouseModel.Slots[removeProductPopupViewModel.SlotIndex].RemoveProduct();
                audioManader.PlaySound(SoundNames.Remove);
            }
            else if (popupModel is NotifyInactiveFriendPopupViewModel notifyInactiveFriendPopupViewModel)
            {
                dispatcher.RequestNotifyInactiveFriend(notifyInactiveFriendPopupViewModel.FriendUid);
            }

            if (popupModel is IConfirmRemoveWithRefundPopupViewModel confirmRemoveWithRefundViewModel)
            {
                playerModel.AddCash(confirmRemoveWithRefundViewModel.SellPrice);
                dispatcher.UIRequestFlyingPrice(screenCalculator.CellToScreenPoint(coords), false, confirmRemoveWithRefundViewModel.SellPrice);
            }
        }
    }
}
