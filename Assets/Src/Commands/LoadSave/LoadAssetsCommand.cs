using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Managers;
using Src.Model;
using UnityEngine;
using UnityEngine.U2D;

namespace Src.Commands.LoadSave
{
    public struct LoadAssetsCommand : IAsyncGameLoadCommand
    {
        public async UniTask<bool> ExecuteAsync()
        {
            var mainConfig = GameConfigManager.Instance.MainConfig;
            var graphicsManager = GraphicsManager.Instance;
            var loadGameProgressModel = LoadGameProgressModel.Instance;

            var loadProgressProxy = new LoadPartsProgressProxy(loadGameProgressModel.SetCurrentPartLoadProgress);
            var loadGameplayGraphicsBundleTask = LoadAsync(AssetBundleNames.GRAPHICS_GAMEPLAY, mainConfig.GameplayAtlasVersion, loadProgressProxy);
            var loadInterfaceGraphicsBundleTask = LoadAsync(AssetBundleNames.GRAPHICS_INTERFACE, mainConfig.InterfaceAtlasVersion, loadProgressProxy);
            var loadAudioBundleTask = LoadAsync(AssetBundleNames.AUDIO, mainConfig.AudioBundleVersion, loadProgressProxy);

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
            AudioManager.Instance.SetSounds(sounds);

            loadProgressProxy.Dispose();

            return true;
        }

        private UniTask<AssetBundle> LoadAsync(string bundleName, int version, LoadPartsProgressProxy progressProxy)
        {
            progressProxy.AddPart(bundleName);
            return AssetBundlesLoader.Instance.LoadOrGetBundle(
                bundleName,
                version,
                p => progressProxy.UpdateProgress(bundleName, p));
        }
    }

    public class LoadPartsProgressProxy : IDisposable
    {
        private Action<float> _progressCallback;

        private List<LoadProgressItem> _progressParts = new List<LoadProgressItem>(3);

        public LoadPartsProgressProxy(Action<float> progressCallback)
        {
            _progressCallback = progressCallback;
        }

        public void AddPart(string name)
        {
            foreach (var itemTmp in _progressParts)
            {
                if (itemTmp.Name == name)
                {
                    return;
                }
            }
            var item = new LoadProgressItem(name);
            _progressParts.Add(item);
        }

        public void UpdateProgress(string name, float progress)
        {
            LoadProgressItem item = null;
            foreach (var itemTmp in _progressParts)
            {
                if (itemTmp.Name == name)
                {
                    item = itemTmp;
                    break;
                }
            }
            if (item == null)
            {
                item = new LoadProgressItem(name);
                _progressParts.Add(item);
            }

            item.Progress = progress;

            var totalProgress = 0f;
            foreach (var itemTmp in _progressParts)
            {
                totalProgress += itemTmp.Progress;
            }
            _progressCallback(totalProgress / _progressParts.Count);
        }

        public void Dispose()
        {
            _progressCallback = null;
        }
    }

    public class LoadProgressItem
    {
        public readonly string Name;
        public float Progress;

        public LoadProgressItem(string name)
        {
            Name = name;
        }
    }
}