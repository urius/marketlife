using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public struct LoadGraphicsCommand
{
    public async UniTask ExecuteAsync()
    {
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var loadGameplayGraphicsBundleTask = AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.GRAPHICS_GAMEPLAY, mainConfig.GameplayAtlasVersion);
        var loadInterfaceGraphicsBundleTask = AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.GRAPHICS_INTERFACE, mainConfig.InterfaceAtlasVersion);

        var bundle = await loadGameplayGraphicsBundleTask;
        var atlas = bundle.LoadAsset<SpriteAtlas>("GameplayGraphicsAtlas");
        var sprites = bundle.LoadAllAssets<Sprite>();
        GraphicsManager.Instance.SetupAtlas(SpriteAtlasId.GameplayAtlas, atlas, sprites);

        bundle = await loadInterfaceGraphicsBundleTask;
        atlas = bundle.LoadAsset<SpriteAtlas>("InterfaceGraphicsAtlas");
        sprites = bundle.LoadAllAssets<Sprite>();
        GraphicsManager.Instance.SetupAtlas(SpriteAtlasId.InterfaceAtlas, atlas, sprites);
    }
}
