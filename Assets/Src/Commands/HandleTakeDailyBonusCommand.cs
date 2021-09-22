using Cysharp.Threading.Tasks;
using UnityEngine;

public struct HandleTakeDailyBonusCommand
{
    public async void Execute(Vector3[] itemsWorldPositions)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var dailyBonusPopupViewModel = gameStateModel.ShowingPopupModel as DailyBonusPopupViewModel;
        var bonusConfig = GameConfigManager.Instance.DailyBonusConfig;
        var dispatcher = Dispatcher.Instance;
        var screenCalculator = ScreenCalculator.Instance;

        var bonusConfigItems = bonusConfig.DailyBonusConfig;
        for (var i = 0; i < bonusConfigItems.Length; i++)
        {
            var bonusConfigItem = bonusConfigItems[i];
            if (dailyBonusPopupViewModel.CurrentBonusDay >= bonusConfigItem.DayNum)
            {
                var screenPoint = screenCalculator.WorldToScreenPoint(itemsWorldPositions[i]);
                if (bonusConfigItem.Reward.IsGold)
                {
                    dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, bonusConfigItem.Reward.Value);
                }
                else
                {
                    dispatcher.UIRequestAddCashFlyAnimation(screenPoint, 5 * (i + 1));
                }
            }
        }

        var reward = bonusConfig.GetDailyBonusRewardForDay(dailyBonusPopupViewModel.CurrentBonusDay);
        if (reward.CashAmount > 0)
        {
            playerModel.AddCash(reward.CashAmount);
        }
        if (reward.GoldAmount > 0)
        {
            playerModel.AddGold(reward.GoldAmount);
        }

        playerModel.UpdateDailyBonus(dailyBonusPopupViewModel.OpenTimestamp, dailyBonusPopupViewModel.CurrentBonusDay);

        await UniTask.Delay(3000);

        if (gameStateModel.ShowingPopupModel != null && gameStateModel.ShowingPopupModel.PopupType == PopupType.DailyBonus)
        {
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
    }
}
