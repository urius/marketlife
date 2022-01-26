using System.Collections.Generic;
using Newtonsoft.Json;

public struct SetVkFriendsDataCommand
{
    public void Execute(string friendsDataStr)
    {
        var deserializedData = JsonConvert.DeserializeObject<VkFriendsDataDto>(friendsDataStr);
        var friendDatas = ToFriendsData(deserializedData.data);

        new SetupFriendsDataCommand().Execute(friendDatas);
    }

    private FriendData[] ToFriendsData(IList<VkFriendDataDto> items)
    {
        var count = items.Count;
        var result = new FriendData[count];
        for (var i = 0; i < count; i++)
        {
            var item = items[i];
            result[i] = new FriendData(item.id.ToString(), item.is_app, item.first_name, item.last_name, item.photo_50);
        }

        return result;
    }
}

public struct VkFriendsDataDto
{
    public List<VkFriendDataDto> data;
}

public struct VkFriendDataDto
{
    public int id;
    public bool is_app;
    public string first_name;
    public string last_name;
    public string photo_50;
}

