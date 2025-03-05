using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Managers;
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
            var bonusConfig = GameConfigManager.Instance.DailyBonusConfig;
            var dispatcher = Dispatcher.Instance;
            var screenCalculator = ScreenCalculator.Instance;
            var advertStateModel = AdvertViewStateModel.Instance;

            if (gameStateModel.ShowingPopupModel != null && gameStateModel.ShowingPopupModel.PopupType == PopupType.DailyBonus)
            {
                var dailyBonusPopupViewModel = gameStateModel.ShowingPopupModel as DailyBonusPopupViewModel;
                var lastBonusTakeTime = playerModel.BonusState.LastBonusTakeTimestamp;
                if (DateTimeHelper.IsSameDays(lastBonusTakeTime, dailyBonusPopupViewModel.OpenTimestamp) == false)
                {
                    var bonusConfigItems = bonusConfig.DailyBonusConfig;
                    var totalCashReward = 0;
                    var totalGoldReward = 0;
                    for (var i = 0; i < bonusConfigItems.Length; i++)
                    {
                        var bonusConfigItem = bonusConfigItems[i];
                        if (dailyBonusPopupViewModel.CurrentBonusDay >= bonusConfigItem.DayNum)
                        {
                            var screenPoint = screenCalculator.WorldToScreenPoint(itemsWorldPositions[i]);
                            var advertTarget = advertStateModel.GetAdvertTargetTypeByDailyBonusDayNum(bonusConfigItem.DayNum);
                            var isDoubled = advertTarget != AdvertTargetType.None && advertStateModel.IsWatched(advertTarget);
                            if (bonusConfigItem.Reward.IsGold)
                            {
                                totalGoldReward += bonusConfigItem.Reward.Value * (isDoubled ? 2 : 1);
                                dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, bonusConfigItem.Reward.Value);
                            }
                            else
                            {
                                totalCashReward += bonusConfigItem.Reward.Value * (isDoubled ? 2 : 1);
                                dispatcher.UIRequestAddCashFlyAnimation(screenPoint, 5 * (i + 1));
                            }
                            advertStateModel.ResetTarget(advertTarget);
                        }
                    }

                    if (totalCashReward > 0)
                    {
                        playerModel.AddCash(totalCashReward);
                    }
                    if (totalGoldReward > 0)
                    {
                        playerModel.AddGold(totalGoldReward);
                    }

                    playerModel.UpdateDailyBonus(dailyBonusPopupViewModel.OpenTimestamp, dailyBonusPopupViewModel.CurrentBonusDay);

                    await UniTask.Delay(2000);

                    gameStateModel.RemoveCurrentPopupIfNeeded();
                }
            }
        }
    }
}
