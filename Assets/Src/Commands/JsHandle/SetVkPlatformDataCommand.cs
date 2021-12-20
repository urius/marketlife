using Newtonsoft.Json;

public struct SetVkPlatformDataCommand
{
    public void Execute(string dataStr)
    {
        var playerModelHolder = PlayerModelHolder.Instance;

        var dataDto = JsonConvert.DeserializeObject<JsVkPlatfomDataCommandDto>(dataStr);
        playerModelHolder.SetSocialType(GetSocialType(dataDto.data.platform));
        playerModelHolder.SetUid(dataDto.data.viewer_id);
        playerModelHolder.SetBuyInBankAllowed(dataDto.data.is_ios_direct == false);
    }

    private SocialType GetSocialType(string platform)
    {
        return platform switch
        {
            "vk" => SocialType.VK,
            _ => SocialType.Undefined,
        };
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
