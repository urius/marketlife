using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct LoadPlayerShopCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var playerModel = PlayerModel.Instance;
        var url = string.Format(URLsHolder.Instance.GetDataURL, playerModel.Uid);

        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetDataOldResponseDto>(resultOperation.Result);
            var shopModel = result.ToShopModel();
            playerModel.SetShopModel(shopModel);

            Dispatcher.Instance.PlayerShopLoaded(shopModel);
        }

        return resultOperation.IsSuccess;
    }
}
