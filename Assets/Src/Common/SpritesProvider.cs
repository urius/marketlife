using System;
using UnityEngine;

public class SpritesProvider
{
    public static SpritesProvider Instance => _instance.Value;
    private static Lazy<SpritesProvider> _instance = new Lazy<SpritesProvider>();

    private GraphicsManager _graphicsManager;

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

    public Sprite GetShelfIcon(int shelfId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Icon_Shelf_{shelfId}");
    }

    public Sprite GetFloorIcon(int numericId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Floor{numericId}");
    }

    public Sprite GetWallIcon(int numericId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Wall{numericId}");
    }

    public Sprite GetWindowIcon(int numericId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Window{numericId}");
    }

    public Sprite GetDoorIcon(int numericId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Door{numericId}");
    }

    public Sprite GetGoldIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "icon_crystal");
    }

    public Sprite GetCashIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "icon_dollar");
    }

    private Sprite GetSprite(SpriteAtlasId gameplayAtlas, string name)
    {
        if (_graphicsManager == null)
        {
            _graphicsManager = GraphicsManager.Instance;
        }

        return _graphicsManager.GetSprite(gameplayAtlas, name);
    }
}