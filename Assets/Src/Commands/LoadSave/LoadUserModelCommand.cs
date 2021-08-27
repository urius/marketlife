using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public struct LoadUserModelCommand
{
    public async UniTask<UserModel> ExecuteAsync(string uid)
    {
        UserModel result = null;
        var url = string.Format(URLsHolder.Instance.GetDataURL, uid);
        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            var dataImporter = DataImporter.Instance;
            var responseDto = JsonConvert.DeserializeObject<CommonResponseDto>(resultOperation.Result);
            switch (responseDto.v)
            {
                case 0:
                    var deserializedDataOld = JsonConvert.DeserializeObject<GetDataOldResponseDto>(resultOperation.Result);
                    result = dataImporter.ImportOld(deserializedDataOld);
                    break;
                case 1:
                    if (ValidateHash(responseDto.response, responseDto.hash))
                    {
                        var deserializedData = JsonConvert.DeserializeObject<GetDataResponseDto>(responseDto.response);
                        result = dataImporter.Import(deserializedData);
                    }
                    else
                    {
                        Debug.Log($"{nameof(LoadUserModelCommand)}: invalid data hash");
                    }
                    break;
            }
        }

        return result;
    }

    private bool ValidateHash(string response, string hash)
    {
        return hash == MD5Helper.MD5Hash(response);
    }
}
