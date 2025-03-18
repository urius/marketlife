using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Src.Managers
{
    public class AvatarsManager
    {
        public static AvatarsManager Instance => _instance.Value;
        private static Lazy<AvatarsManager> _instance = new Lazy<AvatarsManager>();

        public event Action<string> AvatarLoadedForId = delegate { };
        public event Action<string> AvatarLoadedForUrl = delegate { };

        private readonly Dictionary<string, AvatarDataWithUid> _avatarsDataByUid = new();
        private readonly Dictionary<string, AvatarData> _avatarsDataByUrl = new();

        public void SetupAvatarSettings(string uid, string url)
        {
            _avatarsDataByUid[uid] = new AvatarDataWithUid(uid, url);
        }

        public Sprite GetAvatarSpriteByUid(string uid)
        {
            if (_avatarsDataByUid.TryGetValue(uid, out var data))
            {
                return data.AvatarSprite;
            }

            return null;
        }

        public Sprite GetAvatarSpriteByUrl(string url)
        {
            if (_avatarsDataByUrl.TryGetValue(url, out var data))
            {
                return data.AvatarSprite;
            }

            return null;
        }

        public async void LoadAvatarForUid(string uid)
        {
            if (_avatarsDataByUid.TryGetValue(uid, out var avatarData))
            {
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

        public async void LoadAvatarForUrl(string url)
        {
            if (_avatarsDataByUrl.ContainsKey(url) == false)
            {
                var avatarData = new AvatarData(url);
                _avatarsDataByUrl[url] = avatarData;
                
                if (avatarData.DownloadIsInProgress == false)
                {
                    avatarData.DownloadIsInProgress = true;
                    using (var webRequest = UnityWebRequestTexture.GetTexture(avatarData.Url))
                    {
                        await webRequest.SendWebRequest();
                        if (webRequest.result == UnityWebRequest.Result.Success)
                        {
                            avatarData.SetLoadedData(webRequest.downloadHandler.data);
                            AvatarLoadedForUrl(avatarData.Url);
                        }
                    }

                    avatarData.DownloadIsInProgress = false;
                }
            }
        }

        private class AvatarData
        {
            public readonly string Url;

            public bool DownloadIsInProgress = false;

            public AvatarData(string url)
            {
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

        private class AvatarDataWithUid : AvatarData
        {
            public readonly string Uid;

            public AvatarDataWithUid(string uid, string url)
                : base(url)
            {
                Uid = uid;
            }
        }
    }
}