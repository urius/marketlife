using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "GraphicsManager", menuName = "Scriptable Objects/Managers/GraphicsManager")]
public class GraphicsManager : ScriptableObject
{
    public static GraphicsManager Instance { get; private set; }

    private readonly Dictionary<SpriteAtlasId, SpriteAtlasData> _atlasDataById = new Dictionary<SpriteAtlasId, SpriteAtlasData>();

    public void SetupAtlas(SpriteAtlasId atlasId, SpriteAtlas atlas, Sprite[] sprites)
    {
        var atlasData = new SpriteAtlasData(atlas, sprites);
        _atlasDataById[atlasId] = atlasData;
    }

    public Sprite GetHumanSprite(string name)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, name);
    }

    public Sprite GetFloorSprite(int floorId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Floor{floorId}");
    }

    public Sprite GetWallSprite(int wallId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Wall{wallId}");
    }

    public Sprite GetWindowSprite(int windowId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Window{windowId}");
    }

    public Sprite GetDoorSprite(int doorId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Door{doorId}");
    }

    public Sprite GetSprite(SpriteAtlasId atlasId, string spriteName)
    {
        if (TryGetAtlasData(atlasId, out var atlasData))
        {
            if (atlasData.Sprites.TryGetValue(spriteName, out var sprite))
            {
                return sprite;
            }
            else
            {
                Debug.Log($"[ GraphicsManager ] GetSprite: unable to find sprite {spriteName} in atlas {atlasId.ToString()}");
            }
        }

        return null;
    }

    public Sprite[] GetAllIncrementiveSprites(SpriteAtlasId atlasId, string spriteNamePrefix, string spriteNamePostfix = "")
    {
        var result = new List<Sprite>();
        if (TryGetAtlasData(atlasId, out var atlasData))
        {
            var spriteNum = 1;
            while (atlasData.Sprites.TryGetValue($"{spriteNamePrefix}{spriteNum}{spriteNamePostfix}", out var sprite))
            {
                result.Add(sprite);
                spriteNum++;
            }
        }

        return result.ToArray();
    }

    private bool TryGetAtlasData(SpriteAtlasId atlasId, out SpriteAtlasData atlasData)
    {
        if (_atlasDataById.TryGetValue(atlasId, out atlasData))
        {
            return true;
        }
        else
        {
            Debug.Log($"[ GraphicsManager ] GetSprite: unable to atlas {atlasId.ToString()}");
        }

        return false;
    }

    private void OnEnable()
    {
        Instance = this;

        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
    }

    private void OnAtlasRequested(string atlasName, Action<SpriteAtlas> callback)
    {
        var atlasData = _atlasDataById.Values.FirstOrDefault(v => v.Atlasname == atlasName);
        if (atlasData != null)
        {
            callback(atlasData.Atlas);
        }
        else
        {
            Debug.Log($"[ GraphicsManager ] OnAtlasRequested: Unable to find Atlas {atlasName}");
        }
    }
}

public class SpriteAtlasData
{
    public readonly SpriteAtlas Atlas;
    public readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

    public SpriteAtlasData(SpriteAtlas atlas, Sprite[] sprites)
    {
        Atlas = atlas;
        foreach (var sprite in sprites)
        {
            Sprites[sprite.name] = sprite;
        }
    }

    public string Atlasname => Atlas.name;
}

public enum SpriteAtlasId
{
    Undefined,
    GameplayAtlas,
}
