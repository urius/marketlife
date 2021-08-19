using Newtonsoft.Json;

public struct SetVkFriendsDataCommand
{
    public void Execute(string friendsDataStr)
    {
        var friendsDataHolder = FriendsDataHolder.Instance;
        var avatarsManager = AvatarsManager.Instance;
        var deserialisedData = JsonConvert.DeserializeObject<VkFriendsDataDto>(friendsDataStr);

        var friendsData = ToFriendsData(deserialisedData.response.items);
        friendsDataHolder.SetupFriendsData(friendsData);
        foreach (var friendData in friendsData)
        {
            avatarsManager.SetupAvatarSettings(friendData.Uid, friendData.Picture50Url);
        }
    }

    private FriendData[] ToFriendsData(VkFriendDataDto[] items)
    {
        var result = new FriendData[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            result[i] = new FriendData(item.id.ToString(), item.is_app, item.first_name, item.last_name, item.photo_50);
        }

        return result;
    }
}

public class VkFriendsDataDto
{
    public VkFriendsDataResponseDto response;
}

public class VkFriendsDataResponseDto
{
    public VkFriendDataDto[] items;
}

public class VkFriendDataDto
{
    public int id;
    public bool is_app;
    public string first_name;
    public string last_name;
    public string photo_50;
}

