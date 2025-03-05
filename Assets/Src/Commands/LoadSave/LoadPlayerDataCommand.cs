using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Model;
using Src.Net;

namespace Src.Commands.LoadSave
{
    public struct LoadPlayerDataCommand : IAsyncGameLoadCommand
    {
        public async UniTask<bool> ExecuteAsync()
        {
            var result = false;
            var playerModelHolder = PlayerModelHolder.Instance;

            var playerDataStr = await new LoadUserDataCommand().ExecuteAsync(playerModelHolder.Uid);
            if (playerDataStr != null)
            {
                playerModelHolder.SetUserDataRaw(playerDataStr);
                InitializeABData(playerDataStr);

                result = true;
            }

            return result;
        }

        private void InitializeABData(string playerDataStr)
        {
            var responseInnerStr = JsonConvert.DeserializeObject<CommonResponseDto>(playerDataStr).response;
            var abResponseDto = JsonConvert.DeserializeObject<GetDataResponseABDto>(responseInnerStr);
            var abDto = abResponseDto.data.ab;

            ABDataHolder.Instance.Setup(mainConfigPostfix: abDto.config_postfix);
        }
    }
}
