using Newtonsoft.Json;

public struct ProcessShowAdsResultCommand
{
    public void Execute(string message)
    {
        var gameStateModel = GameStateModel.Instance;
        var advertViewStateModel = AdvertViewStateModel.Instance;

        var deserialized = JsonConvert.DeserializeObject<JsShowAdsResultCommandDto>(message);
        if (deserialized.data.is_success == true
            && gameStateModel.ShowingPopupModel != null)
        {
            switch (gameStateModel.ShowingPopupModel.PopupType)
            {
                case PopupType.OfflineReport:
                    var offlineReportPopupViewModel = gameStateModel.ShowingPopupModel as OfflineReportPopupViewModel;
                    advertViewStateModel.ChargeReward(new Price(offlineReportPopupViewModel.TotalProfitFromSell, isGold: false));
                    break;
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
