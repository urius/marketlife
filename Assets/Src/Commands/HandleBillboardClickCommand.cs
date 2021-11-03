using UnityEngine;

public class HandleBillboardClickCommand : MonoBehaviour
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;

        if (gameStateModel.IsPlayingState
            && gameStateModel.ActionState == ActionStateName.None
            && gameStateModel.ShowingPopupModel == null)
        {
            gameStateModel.ShowPopup(new BillboardPopupViewModel(playerModelHolder.ShopModel.BillboardModel));
        }
    }
}
