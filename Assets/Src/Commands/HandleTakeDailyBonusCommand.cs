using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Helpers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.Commands
{
    public struct HandleTakeDailyBonusCommand
    {
        public async void Execute(Vector3[] itemsWorldPositions)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var gameStateModel = GameStateModel.Instance;
            var dispatcher = Dispatcher.Instance;
            var screenCalculator = ScreenCalculator.Instance;
            var advertStateModel = AdvertViewStateModel.Instance;

            if (gameStateModel.ShowingPopupModel != null && gameStateModel.ShowingPopupModel.PopupType == PopupType.DailyBonus)
            {
                var dailyBonusPopupViewModel = gameStateModel.ShowingPopupModel as DailyBonusPopupViewModel;
                var lastBonusTakeTime = playerModel.BonusState.LastBonusTakeTimestamp;
                if (DateTimeHelper.IsSameDays(lastBonusTakeTime, dailyBonusPopupViewModel.OpenTimestamp) == false)
                {
                    var targetDayItemIndex = DailyBonusRewardHelper.GetDayItemIndex(dailyBonusPopupViewModel.CurrentBonusDay);
                    var reward = DailyBonusRewardHelper.GetRewardForDay(dailyBonusPopupViewModel.CurrentBonusDay);
                    var screenPoint = screenCalculator.WorldToScreenPoint(itemsWorldPositions[targetDayItemIndex]);
                    if (reward.IsGold)
                    {
                        dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, reward.Value);
                        playerModel.AddGold(reward.Value);
                    }
                    else
                    {
                        dispatcher.UIRequestAddCashFlyAnimation(screenPoint, 5 * (targetDayItemIndex + 1));
                        playerModel.AddCash(reward.Value);
                    }

                    advertStateModel.ResetTarget(AdvertTargetType.DailyBonusRewardX2);
                    playerModel.UpdateDailyBonus(dailyBonusPopupViewModel.OpenTimestamp,
                        dailyBonusPopupViewModel.CurrentBonusDay);

                    await UniTask.Delay(2000);

                    gameStateModel.RemoveCurrentPopupIfNeeded();
                }
            }
        }
    }
}
