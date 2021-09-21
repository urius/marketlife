using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct LoadCompensationDataCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        if (playerModel.BonusState.IsOldGameBonusProcessed == false)
        {
            var url = string.Format(URLsHolder.Instance.GetFriendDataOldURL, playerModel.Uid);
            var resultOperation = await new WebRequestsSender().GetAsync(url);
            if (resultOperation.IsSuccess)
            {
                var deserializedDataOldCommon = JsonConvert.DeserializeObject<GetDataOldCommonResponseDto>(resultOperation.Result);
                if (int.TryParse(deserializedDataOldCommon.response, out var responseCode))
                {
                    if (responseCode == 1)
                    {
                        var dataImporter = DataImporter.Instance;
                        var deserializedDataOld = JsonConvert.DeserializeObject<GetDataOldResponseDto>(resultOperation.Result);
                        var oldGameUserModel = dataImporter.ImportOld(deserializedDataOld);
                        var oldLevel = oldGameUserModel.ProgressModel.Level;
                        var shopSquare = oldGameUserModel.ShopModel.ShopDesign.SizeX * oldGameUserModel.ShopModel.ShopDesign.SizeY;

                        var compensationHolder = OldGameCompensationHolder.Instance;
                        compensationHolder.SetupCompensation(new OldGameCompensation()
                        {
                            AmountCash = deserializedDataOld.data.cash_b + shopSquare * 100,
                            AmountGold = deserializedDataOld.data.gold_b + oldLevel,
                        });
                    }
                    else if (responseCode == -1)
                    {
                        playerModel.MarkOldGameBonusProcessed();
                    }

                    return true;
                }
            }

            return false;
        }

        return true;
    }
}
