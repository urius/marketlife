using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.Commands.JsHandle
{
    public struct ProcessShowRewardedAdsResultCommand
    {
        public void Execute(bool isSuccessResult)
        {
            var gameStateModel = GameStateModel.Instance;
            var advertViewStateModel = AdvertViewStateModel.Instance;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var mainConfig = GameConfigManager.Instance.MainConfig;

            Debug.Log("ProcessShowRewardedAdsResultCommand: result = " + isSuccessResult);

            advertViewStateModel.LastAdvertWatchTime = gameStateModel.ServerTime;
            
            if (isSuccessResult)
            {
                advertViewStateModel.MarkCurrentAsWatched();

                if (gameStateModel.ShowingPopupModel != null)
                {
                    switch (gameStateModel.ShowingPopupModel.PopupType)
                    {
                        case PopupType.Bank:
                            if (advertViewStateModel.IsWatched(AdvertTargetType.BankGold))
                            {
                                playerModel.AddGold(mainConfig.BankAdvertRewardGold);
                                advertViewStateModel.ResetTarget(AdvertTargetType.BankGold);
                            }
                            else if (advertViewStateModel.IsWatched(AdvertTargetType.BankCash))
                            {
                                playerModel.AddCash(mainConfig.BankAdvertRewardGold * CalculationHelper.GetGoldToCashConversionRate());
                                advertViewStateModel.ResetTarget(AdvertTargetType.BankCash);
                            }

                            advertViewStateModel.BankAdvertWatchTime = gameStateModel.ServerTime;
                            advertViewStateModel.BankAdvertWatchesCount++;
                            //gameStateModel.RemoveCurrentPopupIfNeeded();
                            break;
                    }
                }
                else
                {
                    advertViewStateModel.MarkCurrentAsCanceled();
                }
            }
        }
    }
}