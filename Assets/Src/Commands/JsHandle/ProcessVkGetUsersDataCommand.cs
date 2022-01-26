using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public struct ProcessVkGetUsersDataCommand
{
    public void Execute(string messsage)
    {
        Debug.Log($"{nameof(ProcessVkGetUsersDataCommand)}: message: {messsage}");

        var avatarsManager = AvatarsManager.Instance;
        var deserializedData = JsonConvert.DeserializeObject<VkUsersDataDto>(messsage);
        var convertedData = deserializedData.data.Select(d => new UserSocialData(d.id.ToString(), d.first_name, d.last_name, d.photo_50));
        foreach (var data in convertedData)
        {
            avatarsManager.SetupAvatarSettings(data.Uid, data.Picture50Url);
        }
        SocialUsersData.Instance.FillRequestedSocialData(convertedData);
    }
}

public struct VkUsersDataDto
{
    public List<VkUserDataDto> data;
}

public struct VkUserDataDto
{
    public int id;
    public string first_name;
    public string last_name;
    public string photo_50;
}
