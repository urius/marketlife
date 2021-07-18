using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public struct LoadAssetsCommand
{
    public async UniTask ExecuteAsync()
    {
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var graphicsManager = GraphicsManager.Instance;
        var loadGameplayGraphicsBundleTask = AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.GRAPHICS_GAMEPLAY, mainConfig.GameplayAtlasVersion);
        var loadInterfaceGraphicsBundleTask = AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.GRAPHICS_INTERFACE, mainConfig.InterfaceAtlasVersion);
        var loadAudioBundleTask = AssetBundlesLoader.Instance.LoadOrGetBundle(AssetBundleNames.AUDIO, mainConfig.AudioBundleVersion);

        var bundle = await loadGameplayGraphicsBundleTask;
        var atlas = bundle.LoadAsset<SpriteAtlas>("GameplayGraphicsAtlas");
        var sprites = bundle.LoadAllAssets<Sprite>();
        graphicsManager.SetupAtlas(SpriteAtlasId.GameplayAtlas, atlas, sprites);

        bundle = await loadInterfaceGraphicsBundleTask;
        atlas = bundle.LoadAsset<SpriteAtlas>("InterfaceGraphicsAtlas");
        sprites = bundle.LoadAllAssets<Sprite>();
        graphicsManager.SetupAtlas(SpriteAtlasId.InterfaceAtlas, atlas, sprites);

        var starsPSPrefab = bundle.LoadAsset<GameObject>(PrefabsHolder.PSStarsName);
        PrefabsHolder.Instance.SetupRemotePrefab(PrefabsHolder.PSStarsName, starsPSPrefab);

        bundle = await loadAudioBundleTask;
        var sounds = bundle.LoadAllAssets<AudioClip>();
    }
}
