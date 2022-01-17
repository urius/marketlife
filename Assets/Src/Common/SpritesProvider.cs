using System;
using UnityEngine;

public class SpritesProvider
{
    public static SpritesProvider Instance => _instance.Value;
    private static Lazy<SpritesProvider> _instance = new Lazy<SpritesProvider>();

    public const string HUMAN_HAIR_PREFIX = "Hair";
    public const string HUMAN_GLASSES_PREFIX = "Glasses";
    public const string CLOTHES_PREFIX = "Clothes";
    public const string HAND_CLOTHES_PREFIX = "HandClothes";

    private GraphicsManager _graphicsManager;
    private int _hairNumMax;
    private int _glassesNumMax;
    private int _topClothesNumMax;
    private int _bottomClothesNumMax;

    public Sprite GetHumanSprite(string name)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, name);
    }

    public Sprite GetHumanHairSprite(int hairId)
    {
        return GetHumanSprite($"{HUMAN_HAIR_PREFIX}{hairId}");
    }

    public Sprite GetHumanGlassesSprite(int glassId)
    {
        return GetHumanSprite($"{HUMAN_GLASSES_PREFIX}{glassId}");
    }

    public Sprite GetTopDressSprite(int dressId)
    {
        return GetHumanSprite($"{CLOTHES_PREFIX}{dressId}");
    }

    public Sprite GetHandDressSprite(int dressId)
    {
        return GetHumanSprite($"{HAND_CLOTHES_PREFIX}{dressId}");
    }

    public Sprite GetFloorSprite(int floorId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Floor{floorId}");
    }

    public Sprite GetGrassSprite(int grassId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"FloorGrass{grassId}");
    }

    public Sprite GetSnowSprite(int snowId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"FloorSnow{snowId}");
    }

    public Sprite GetSantaHatSprite()
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"SantaHat");
    }

    public Sprite GetExclamationMarkSprite()
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"ExclamationMark");
    }

    public Sprite GetTreeSprite()
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Tree");
    }

    public Sprite GetWinterTreeSprite()
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"WinterTree");
    }

    public Sprite GetUnwashSprite(int numericId)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, $"Unwashes{numericId}");
    }

    public Sprite GetRandomUnwashIcon()
    {
        return GetUnwashIcon(UnityEngine.Random.Range(1, 4));
    }

    public Sprite GetUnwashIcon(int numericId)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, $"unwashes_icon_{numericId}");
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

    //public Sprite GetWallIcon(int numericId)
    //{
    //    return GetSprite(SpriteAtlasId.InterfaceAtlas, $"Wall{numericId}");
    //}

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

    public Sprite GetBlueButtonSprite()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "button_1_long_blue");
    }

    public Sprite GetCrimsonButtonSprite()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "button_1_long_crimson");
    }

    public Sprite GetGreenButtonSprite()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "button_1_long_green");
    }

    public Sprite GetOrangeButtonSprite()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "button_1_long_orange");
    }

    public Sprite GetOrderIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "Icon_Order");
    }

    public Sprite GetProductIcon(string productKey)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, productKey);
    }

    public Sprite GetProductSprite(string productKey)
    {
        return GetSprite(SpriteAtlasId.GameplayAtlas, productKey);
    }

    public Sprite GetPersonalIcon(string key)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, key);
    }

    public Sprite GetStarIcon(bool isBig)
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, isBig ? "icon_star_big" : "icon_star_05");
    }

    public Sprite GetFriendsIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_friends");
    }

    public Sprite GetTopStribeGreen()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "skin_top_stribe_green");
    }

    public Sprite GetTopStribeRed()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "skin_top_stribe_red");
    }

    public Sprite GetTopStribeYellow()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "skin_top_stribe_yellow");
    }

    public Sprite GetUpgradeIcon(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.ExpandX => GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_expand_shop_x"),
            UpgradeType.ExpandY => GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_expand_shop_y"),
            UpgradeType.WarehouseSlots => GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_upgrade_wh_slot"),
            UpgradeType.WarehouseVolume => GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_upgrade_wh_volume"),
            _ => null,
        };
    }

    public Sprite GetTakeActionIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "icon_grab");
    }

    public Sprite GetAddUnwashActionIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "unwashes_icon_1");
    }

    public Sprite GetBigPlusSignIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "sign_plus_big");
    }

    public Sprite GetMoonIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "Icon_Moon");
    }

    public Sprite GetMissionsIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "icon_Checklist");
    }

    public Sprite GetGiftboxBlueIcon()
    {
        return GetSprite(SpriteAtlasId.InterfaceAtlas, "icon_giftbox_blue");
    }

    public int GetMaxHairId()
    {
        if (_hairNumMax <= 0)
        {
            _hairNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, HUMAN_HAIR_PREFIX).Length;
        }
        return _hairNumMax;
    }

    public int GetMaxGlassesId()
    {
        if (_glassesNumMax <= 0)
        {
            _glassesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, HUMAN_GLASSES_PREFIX).Length;
        }
        return _glassesNumMax;
    }

    public int GetMaxTopDressId()
    {
        if (_topClothesNumMax <= 0)
        {
            _topClothesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, CLOTHES_PREFIX).Length;
        }
        return _topClothesNumMax;
    }

    public int GetMaxBottomDressId()
    {
        if (_bottomClothesNumMax <= 0)
        {
            _bottomClothesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, HumanView.FOOT_CLOTHES_PREFIX).Length;
        }
        return _bottomClothesNumMax;
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
