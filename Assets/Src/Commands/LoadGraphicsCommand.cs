using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public struct LoadGraphicsCommand
{
    public async UniTask ExecuteAsync()
    {
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var bundle = await AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.GRAPHICS_GAMEPLAY, mainConfig.GameplayAtlasVersion);

        var atlas = bundle.LoadAsset<SpriteAtlas>("GameplayGraphicsAtlas");
        var sprites = bundle.LoadAllAssets<Sprite>();
        GraphicsManager.Instance.SetupAtlas(SpriteAtlasId.GameplayAtlas, atlas, sprites);
    }
}
