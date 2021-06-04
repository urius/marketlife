using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct LoadServerTimeCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var gameStateModel = GameStateModel.Instance;
        var url = URLsHolder.Instance.GetTimeURL;

        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetTimeResponseDto>(resultOperation.Result);
            var serverTime = int.Parse(result.response);
            gameStateModel.SetServerTime(serverTime);
        }

        return resultOperation.IsSuccess;
    }
}

public struct GetTimeResponseDto
{
    public string response;
}
