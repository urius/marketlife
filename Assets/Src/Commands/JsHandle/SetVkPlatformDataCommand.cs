using Newtonsoft.Json;

public struct SetVkPlatformDataCommand
{
    public void Execute(string data)
    {
        var playerModelHolder = PlayerModelHolder.Instance;

        var dataDto = JsonConvert.DeserializeObject<JsVkPlatfomDataCommandDto>(data);
        playerModelHolder.SetUid(dataDto.data.viewer_id);
    }
}
