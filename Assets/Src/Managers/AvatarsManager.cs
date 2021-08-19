using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarsManager
{
    public static AvatarsManager Instance => _instance.Value;
    private static Lazy<AvatarsManager> _instance = new Lazy<AvatarsManager>();

    public event Action<string> AvatarLoadedForId = delegate { };

    private readonly Dictionary<string, AvatarData> _avatarsData = new Dictionary<string, AvatarData>();

    public void SetupAvatarSettings(string uid, string url)
    {
        _avatarsData[uid] = new AvatarData(uid, url);
    }

    public Sprite GetAvatarSprite(string uid)
    {
        if (_avatarsData.TryGetValue(uid, out var data))
        {
            return data.AvatarSprite;
        }

        return null;
    }

    public async void LoadAvatarForUid(string uid)
    {
        if (_avatarsData.ContainsKey(uid))
        {
            var avatarData = _avatarsData[uid];
            if (avatarData.DownloadIsInProgress == false)
            {
                avatarData.DownloadIsInProgress = true;
                using (var webRequest = UnityWebRequestTexture.GetTexture(avatarData.Url))
                {
                    await webRequest.SendWebRequest();
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        avatarData.SetLoadedData(webRequest.downloadHandler.data);
                        AvatarLoadedForId(avatarData.Uid);
                    }
                }
                avatarData.DownloadIsInProgress = false;
            }
        }
    }
}

public class AvatarData
{
    public readonly string Uid;
    public readonly string Url;

    public bool DownloadIsInProgress = false;

    public AvatarData(string uid, string url)
    {
        Uid = uid;
        Url = url;
    }

    public Sprite AvatarSprite { get; private set; }

    public void SetLoadedData(byte[] data)
    {
        var texture = new Texture2D(2, 2);
        texture.LoadImage(data);
        AvatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
