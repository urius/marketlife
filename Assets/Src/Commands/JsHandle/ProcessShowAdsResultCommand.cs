using Newtonsoft.Json;

public struct ProcessShowAdsResultCommand
{
    public void Execute(string message)
    {
        var gameStateModel = GameStateModel.Instance;
        var advertViewStateModel = AdvertViewStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;

        var deserialized = JsonConvert.DeserializeObject<JsShowAdsResultCommandDto>(message);
        if (deserialized.data.is_success == true
            && gameStateModel.ShowingPopupModel != null)
        {
            advertViewStateModel.ChargeReward();
            switch (gameStateModel.ShowingPopupModel.PopupType)
            {
                case PopupType.Bank:
                    if (advertViewStateModel.Reward.IsGold)
                    {
                        playerModel.AddGold(advertViewStateModel.Reward.Value);
                    }
                    else
                    {
                        playerModel.AddCash(advertViewStateModel.Reward.Value);
                    }
                    advertViewStateModel.ResetChargedReward();
                    gameStateModel.RemoveCurrentPopupIfNeeded();
                    break;
            }
        }
        else
        {
            advertViewStateModel.ResetChargedReward();
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
