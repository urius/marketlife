using Src.Common;
using UnityEngine;

public struct TakeVisitFriendBonusCommand
{
    public void Execute(Vector3 screenCoords)
    {
        var gameStateModel = GameStateModel.Instance;
        var friendModel = gameStateModel.ViewingUserModel;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var actionsConfig = GameConfigManager.Instance.FriendActionsConfig;
        var dispatcher = Dispatcher.Instance;
        var friendActionsConfig = GameConfigManager.Instance.FriendActionsConfig;

        var friendActionsModel = playerModel.FriendsActionsDataModels.GetFriendShopActionsModel(friendModel.Uid);
        var actionId = FriendShopActionId.VisitBonus;
        var visitBonusActionData = friendActionsModel.ActionsById[actionId];
        if (friendActionsConfig.IsEnabled(actionId) && visitBonusActionData.IsAvailable(gameStateModel.ServerTime))
        {
            visitBonusActionData.SetEndCooldownTime(actionsConfig.GetDefaultActionCooldownMinutes(actionId) * 60 + gameStateModel.ServerTime);
            visitBonusActionData.SetAmount(actionsConfig.GetDefaultActionAmount(actionId));

            var addGoldAmount = 1;
            dispatcher.UIRequestAddGoldFlyAnimation(screenCoords, addGoldAmount);
            playerModel.AddGold(addGoldAmount);
        }
    }
}
