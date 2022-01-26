using Newtonsoft.Json;
using UnityEngine;

public struct CreateUserModelCommand
{
    public UserModel Execute(string userDataStr)
    {
        UserModel result = null;
        if (userDataStr != null)
        {
            var dataImporter = DataImporter.Instance;
            var responseDto = JsonConvert.DeserializeObject<CommonResponseDto>(userDataStr);
            switch (responseDto.v)
            {
                case 0:
                    var deserializedDataOld = JsonConvert.DeserializeObject<GetDataOldResponseDto>(userDataStr);
                    result = dataImporter.ImportOld(deserializedDataOld);
                    break;
                case 1:
                    if (ResponseValidator.Validate(responseDto))
                    {
                        var deserializedData = JsonConvert.DeserializeObject<GetDataResponseDto>(responseDto.response);
                        result = dataImporter.Import(deserializedData);
                    }
                    else
                    {
                        Debug.Log($"{nameof(CreateUserModelCommand)}: invalid data hash");
                    }
                    break;
            }
        }

        return result;
    }
}
