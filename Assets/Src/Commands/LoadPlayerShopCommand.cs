using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct LoadPlayerShopCommand
{
    private const string _getDataUrl = "https://devman.ru/marketVK/dataProvider.php?command=get_data&id={0}";

    public async UniTask<bool> ExecuteAsync()
    {
        var playerModel = PlayerModel.Instance;
        var url = string.Format(_getDataUrl, playerModel.Uid);

        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetDataResponseDto>(resultOperation.Result);
            var shopModel = result.ToShopModel();
            playerModel.SetShopModel(shopModel);

            Dispatcher.Instance.PlayerShopLoaded(shopModel);
        }

        return resultOperation.IsSuccess;
    }
}
