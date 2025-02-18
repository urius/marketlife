using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;

public struct LoadServerTimeCommand : IAsyncGameLoadCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var gameStateModel = GameStateModel.Instance;
        var url = Urls.GetTimeURL;

        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<GetTimeResponseDto>(resultOperation.Result);
            gameStateModel.SetServerTime(result.response);
        }

        return resultOperation.IsSuccess;
    }
}

public struct GetTimeResponseDto
{
    public int response;
}
