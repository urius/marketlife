using Newtonsoft.Json;

public struct SetVkPlatformDataCommand
{
    public void Execute(string dataStr)
    {
        var playerModelHolder = PlayerModelHolder.Instance;

        var dataDto = JsonConvert.DeserializeObject<JsVkPlatfomDataCommandDto>(dataStr);
        playerModelHolder.SetUid(dataDto.data.viewer_id);
    }
}

public struct JsVkPlatfomDataCommandDto
{
    public string command;
    public JsVkPlatfomDataCommandDataDto data;
}

public struct JsVkPlatfomDataCommandDataDto
{
    public string viewer_id;
}
