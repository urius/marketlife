using Newtonsoft.Json;
using Src.Model;

namespace Src.Commands.JsHandle
{
    public struct SetVkPlatformDataCommand
    {
        public static void Execute(string dataStr)
        {
            var playerModelHolder = PlayerModelHolder.Instance;

            var dataDto = JsonConvert.DeserializeObject<JsVkPlatfomDataCommandDto>(dataStr);
            playerModelHolder.SetInitialData(dataDto.data.viewer_id, SocialType.VK);
            playerModelHolder.SetBuyInBankAllowed(dataDto.data.is_ios_direct == false);
        }
    }

    public struct JsVkPlatfomDataCommandDto
    {
        public string command;
        public JsVkPlatfomDataCommandDataDto data;
    }

    public struct JsVkPlatfomDataCommandDataDto
    {
        public string platform;
        public string viewer_id;
        public bool is_ios_direct;
    }
}