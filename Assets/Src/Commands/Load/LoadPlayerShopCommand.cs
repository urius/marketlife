using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public struct LoadPlayerShopCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var dataImporter = DataImporter.Instance;
        var url = string.Format(URLsHolder.Instance.GetDataURL, playerModelHolder.Uid);

        var resultOperation = await new WebRequestsSender().GetAsync(url);
        if (resultOperation.IsSuccess)
        {
            UserModel loadedUserModel = null;
            var result = JsonConvert.DeserializeObject<CommonResponseDto>(resultOperation.Result);
            switch (result.v)
            {
                case 0:
                    var deserializedDataOld = JsonConvert.DeserializeObject<GetDataOldResponseDto>(resultOperation.Result);
                    loadedUserModel = dataImporter.ImportOld(deserializedDataOld);
                    break;
                case 1:
                    if (ValidateHash(result.response, result.hash))
                    {
                        var deserializedData = JsonConvert.DeserializeObject<GetDataResponseDto>(result.response);
                        loadedUserModel = dataImporter.Import(deserializedData);
                    }
                    else
                    {
                        Debug.Log("LoadPlayerShopCommand: invalid data hash");
                        return false;
                    }
                    break;

            }
            if (loadedUserModel != null)
            {
                playerModelHolder.SetUserModel(loadedUserModel);
            }
        }

        return resultOperation.IsSuccess;
    }

    private bool ValidateHash(string response, string hash)
    {
        return hash == MD5Helper.MD5Hash(response);
    }
}
