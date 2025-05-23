using Cysharp.Threading.Tasks;
using Src.Common;

public struct LoadUserDataCommand
{
    public async UniTask<string> ExecuteAsync(string uid)
    {
        var url = string.Format(Urls.GetDataURL, uid);
        var resultOperation = await new WebRequestsSender().GetAsync(url);
        
        if (resultOperation.IsSuccess)
        {
            return resultOperation.Result;
        }

        return null;
    }
}
