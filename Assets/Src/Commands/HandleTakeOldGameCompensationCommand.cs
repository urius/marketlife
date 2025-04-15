using Src.Common;
using UnityEngine;

public struct HandleTakeOldGameCompensationCommand
{
    public void Execute(Vector3 takeButtonWorldCoords)
    {
        var dispatcher = Dispatcher.Instance;
        var screenClculator = ScreenCalculator.Instance;
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var compensationData = OldGameCompensationHolder.Instance.Compensation;

        var screenPoint = screenClculator.WorldToScreenPoint(takeButtonWorldCoords);
        dispatcher.UIRequestAddCashFlyAnimation(screenPoint, 10);
        dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, 5);
        playerModel.AddCash(compensationData.AmountCash);
        playerModel.AddGold(compensationData.AmountGold);

        playerModel.MarkOldGameBonusProcessed();
        gameStateModel.RemoveCurrentPopupIfNeeded();
    }
}
