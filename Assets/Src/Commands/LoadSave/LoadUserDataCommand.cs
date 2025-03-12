using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Common_Utils;
using Src.Net;
using UnityEngine;

namespace Src.Commands.LoadSave
{
    public struct LoadUserDataCommand
    {
        public async UniTask<string> ExecuteAsync(string uid)
        {
            if (DisabledLogicFlags.IsServerDataDisabled)
            {
                var playerDataStr =  MirraSdkWrapper.GetString(Constants.PlayerDataKey);

                return string.IsNullOrEmpty(playerDataStr) ? GetDefaultUserDataStr(uid) : playerDataStr;
            }
            else
            {
                var url = string.Format(Urls.GetDataURL, uid);
                var resultOperation = await new WebRequestsSender().GetAsync(url);
                
                if (resultOperation.IsSuccess)
                {
                    var responseDto = JsonConvert.DeserializeObject<CommonResponseDto>(resultOperation.Result);

                    if (Validate(responseDto))
                    {
                        return responseDto.response;
                    }
                }
            }

            return null;
        }

        private string GetDefaultUserDataStr(string uid)
        {
            var defaultUserStr = Resources.Load<TextAsset>("TextAssets/DefaultUser").text;
            
            var userDataDto = JsonConvert.DeserializeObject<UserDataDto>(defaultUserStr);

            var getDataDto = new FullUserDataDto()
            {
                uid = uid,
                data = userDataDto,
                external_data = new ExternalDataDto(),
                first_visit_time = MirraSdkWrapper.GetCurrentTimestampSec(),
            };

            var result = JsonConvert.SerializeObject(getDataDto);

            return result;
        }

        private bool Validate(CommonResponseDto responseDto)
        {
            if (responseDto.v > 0)
            {
                if (ResponseValidator.Validate(responseDto))
                {
                    return true;
                }

                Debug.Log($"{nameof(CreateUserModelCommand)}: invalid data hash");
                
                return false;
            }

            return true;
        }
    }
}
