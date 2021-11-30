using Newtonsoft.Json;

public struct ProcessShowAdsResultCommand
{
    public void Execute(string message)
    {
        var gameStateModel = GameStateModel.Instance;
        var advertViewStateModel = AdvertViewStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;

        var deserialized = JsonConvert.DeserializeObject<JsShowAdsResultCommandDto>(message);
        if (deserialized.data.is_success == true)
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
                        gameStateModel.RemoveCurrentPopupIfNeeded();
                        break;
                }
            }
        }
    }
}

public struct JsShowAdsResultCommandDto
{
    public JsShowAdsResultCommandDataDto data;
}

public struct JsShowAdsResultCommandDataDto
{
    public bool is_success;
}
