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
            LoadResourcesLocal();
            await TryLoadMusicBundle();

            return true;
        }

        private void LoadResourcesLocal()
        {
            var graphicsManager = GraphicsManager.Instance;
            var builtinAssetsHolder = BuiltinAssetsHolderSo.Instance;
            
            graphicsManager.SetupAtlas(SpriteAtlasId.GameplayAtlas, builtinAssetsHolder.BuiltInGameplayAtlas, builtinAssetsHolder.BuiltInGameplayAtlasSprites);
            graphicsManager.SetupAtlas(SpriteAtlasId.InterfaceAtlas, builtinAssetsHolder.BuiltInInterfaceAtlas, builtinAssetsHolder.BuiltInInterfaceAtlasSprites);
            PrefabsHolder.Instance.SetupRemotePrefab(PrefabsHolder.PSStarsName, builtinAssetsHolder.PsStarsPrefab);
            
            AudioManager.Instance.SetSounds(builtinAssetsHolder.BuiltInSounds);
        }

        private async UniTask TryLoadMusicBundle()
        {
            var loadGameProgressModel = LoadGameProgressModel.Instance;
            
            var loadProgressProxy = new LoadPartsProgressProxy(loadGameProgressModel.SetCurrentPartLoadProgress);
            var loadMusicBundleTask = LoadAsync(AssetBundleNames.MUSIC, version: 2, loadProgressProxy);

            try
            {
                var bundle = await loadMusicBundleTask;

                var sounds = bundle.LoadAllAssets<AudioClip>();
                AudioManager.Instance.SetSounds(sounds);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Load music bundle failed: " + e.Message);
            }
            
            loadProgressProxy.Dispose();
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