using System.Threading;
using Src.Managers;
using Src.Model;
using UnityEngine;

namespace Src.Systems
{
    public class MusicControlSystem
    {
        private readonly GameStateModel _gameStateModel;
        private readonly AudioManager _audioManager;
        private readonly PlayerModelHolder _playerModelHolder;

        private CancellationTokenSource _switchMusicCts;

        public MusicControlSystem()
        {
            _gameStateModel = GameStateModel.Instance;
            _audioManager = AudioManager.Instance;
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public void Start()
        {
            _gameStateModel.GameStateChanged += OnGameStateChanged;
        }

        private async void OnGameStateChanged(GameStateName previousState, GameStateName newState)
        {
            var prevMusicType = GetMusicTypeByState(previousState);
            var currentMusicType = GetMusicTypeByState(newState);
            if (prevMusicType != currentMusicType)
            {
                if (_switchMusicCts != null && _switchMusicCts.IsCancellationRequested == false)
                {
                    _switchMusicCts.Cancel();
                }

                _switchMusicCts = new CancellationTokenSource();
                using (_switchMusicCts)
                {
                    await _audioManager.PlayMusicWithFade(_switchMusicCts.Token, GetMusicByType(currentMusicType), 1, 1);
                }
                _switchMusicCts = null;
            }
        }

        private AudioClip GetMusicByType(MusicType musicType)
        {
            if (musicType == MusicType.Game)
            {
                return _playerModelHolder.UserModel.ShopModel.MoodMultiplier < 0.5
                    ? GetAudioOrNull(SoundNames.MusicGameLowMood)
                    : GetAudioOrNull(SoundNames.MusicGameHighMood);
            }

            if (musicType == MusicType.Build)
            {
                return UnityEngine.Random.Range(0, 10) <= 5
                    ? GetAudioOrNull(SoundNames.MusicBuild1)
                    : GetAudioOrNull(SoundNames.MusicBuild2);
            }

            return null;
        }

        private AudioClip GetAudioOrNull(string name)
        {
            if (_audioManager.Sounds.TryGetValue(name, out var audio))
            {
                return audio;
            }

            Debug.LogWarning($"No audio clip found for name: {name}");
            
            return null;
        }

        private MusicType GetMusicTypeByState(GameStateName state)
        {
            return state switch
            {
                GameStateName.ReadyForStart => MusicType.Game,
                GameStateName.PlayerShopSimulation => MusicType.Game,
                GameStateName.ShopFriend => MusicType.Game,
                GameStateName.PlayerShopInterior => MusicType.Build,
                _ => MusicType.None,
            };
        }
    }

    public enum MusicType
    {
        None,
        Game,
        Build,
    }
}