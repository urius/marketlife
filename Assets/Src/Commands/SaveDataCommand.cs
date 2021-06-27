using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct SaveDataCommand
{
    public async UniTask<bool> Execute()
    {
        var playerModel = PlayerModel.Instance;
        var url = string.Format(URLsHolder.Instance.SaveDataURL, playerModel.Uid);

        var exportedDataDto = DataExporter.Instance.ExportFull(playerModel.ShopModel);
        var exportedDataStr = JsonConvert.SerializeObject(exportedDataDto);

        var resultOperation = await new WebRequestsSender().PostAsync<CommonResponseDto>(url, exportedDataStr);

        if (resultOperation.IsSuccess)
        {
            var response = JsonConvert.DeserializeObject<BoolSuccessResponseDto>(resultOperation.Result.response);
            return response.success;
        }
        return resultOperation.IsSuccess;
    }
}
