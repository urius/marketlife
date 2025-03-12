using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Src.Common
{
    [CreateAssetMenu(fileName = "BuiltinAssetsHolderSo", menuName = "Scriptable Objects/Assets/BuiltinAssetsHolderSo")]
    public class BuiltinAssetsHolderSo : ScriptableObject
    {
        public static BuiltinAssetsHolderSo Instance { get; private set; }
        
        [SerializeField] private SpriteAtlas _builtInGameplayAtlas;
        [SerializeField] private Sprite[] _builtInGameplayAtlasSprites;
        [SerializeField] private SpriteAtlas _builtInInterfaceAtlas;
        [SerializeField] private Sprite[] _builtInInterfaceAtlasSprites;
        [SerializeField] private GameObject _psStarsPrefab;
        [SerializeField] private AudioClip[] _builtInSounds;

        public Sprite[] BuiltInGameplayAtlasSprites => _builtInGameplayAtlasSprites;
        public SpriteAtlas BuiltInGameplayAtlas => _builtInGameplayAtlas;
        public Sprite[] BuiltInInterfaceAtlasSprites => _builtInInterfaceAtlasSprites;
        public SpriteAtlas BuiltInInterfaceAtlas => _builtInInterfaceAtlas;
        public GameObject PsStarsPrefab => _psStarsPrefab;
        public AudioClip[] BuiltInSounds => _builtInSounds;

        private void OnEnable()
        {
            Instance = this;
        }

#if UNITY_EDITOR
        [ContextMenu("FillBuiltInSprites")]
        private void FillBuiltInSprites()
        {
            var sprites = LoadAllSpritesInFolder("Assets/Resources/BuiltInAssetBundles/GameplayGraphicsBundle");
            _builtInGameplayAtlasSprites = sprites.ToArray();

            sprites = LoadAllSpritesInFolder("Assets/Resources/BuiltInAssetBundles/InterfaceGraphicsBundle");
            _builtInInterfaceAtlasSprites = sprites.ToArray();

            var sounds = LoadAllAudioClipsInFolder("Assets/Resources/BuiltInAssetBundles/Sounds");

            _builtInSounds = sounds.ToArray();
            
            Debug.Log(sprites.Count);
        }

        private List<Sprite> LoadAllSpritesInFolder(string folderPath)
        {
            var sprites = new List<Sprite>();
            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            return sprites;
        }

        private List<AudioClip> LoadAllAudioClipsInFolder(string folderPath)
        {
            var audioClips = new List<AudioClip>();
            var guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
                if (audioClip != null)
                {
                    audioClips.Add(audioClip);
                }
            }

            return audioClips;
        }
#endif
    }
}