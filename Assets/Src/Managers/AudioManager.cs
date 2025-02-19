using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Dictionary<string, AudioClip> Sounds { get; private set; }

    private GameStateModel _gameStateModel;
    private PlayerModelHolder _playerModelHolder;
    private AudioSource _musicSource;
    private AudioSource _soundsSource;
    private UserSettingsModel _settingsModel;

    public AudioManager()
    {
        Instance = this;
    }

    public void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        SetMusicVolume(0);
        _soundsSource = gameObject.AddComponent<AudioSource>();
        _soundsSource.loop = false;
        SetSoundsVolume(0);
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _settingsModel = _playerModelHolder.UserModel.UserSettingsModel;
        UpdateVolumes();

        Activate();
    }

    public void SetMusicVolume(float volume)
    {
        if (_musicSource != null)
        {
            _musicSource.volume = volume;
        }
    }

    public void SetSoundsVolume(float volume)
    {
        if (_soundsSource != null)
        {
            _soundsSource.volume = volume;
        }
    }

    public void SetSounds(AudioClip[] sounds)
    {
        Sounds = sounds.ToDictionary(s => s.name);
    }

    public void PlaySound(string soundName)
    {
        if (Sounds.ContainsKey(soundName))
        {
            PlaySound(Sounds[soundName]);
        }
        else
        {
            Debug.Log($"[ERROR] AudioManager: can't find sound with name {soundName}");
        }
    }

    public void PlayRandomSound(params string[] soundNames)
    {
        var index = UnityEngine.Random.Range(0, soundNames.Length - 1);
        PlaySound(soundNames[index]);
    }

    public void PlaySound(AudioClip sound)
    {
        _soundsSource?.PlayOneShot(sound);
    }

    public UniTask FadeInAndPlayMusicAsync(CancellationToken stopToken, AudioClip clip, float fadeInDuration = 0.5f)
    {
        if (_musicSource == null) return UniTask.CompletedTask;
        var registration = stopToken.Register(OnCompleteAction);

        void OnCompleteAction()
        {
            LeanTween.cancel(gameObject, callOnComplete: true);
        }

        _musicSource.Stop();
        _musicSource.clip = clip;
        var musicFadeTsc = new UniTaskCompletionSource();
        LeanTween.value(gameObject, f => _musicSource.volume = f, _musicSource.volume, GetMusicVolume(), fadeInDuration)
            .setOnComplete(() =>
            {
                musicFadeTsc.TrySetResult();
                registration.Dispose();
            });
        _musicSource.Play();

        return musicFadeTsc.Task;
    }

    public UniTask FadeOutAndStopMusicAsync(CancellationToken stopToken, float fadeOutDuration = 0.5f)
    {
        if (_musicSource == null) return UniTask.CompletedTask;
        var registration = stopToken.Register(OnCompleteAction);

        void OnCompleteAction()
        {
            LeanTween.cancel(gameObject, callOnComplete: true);
        }

        var musicFadeTsc = new UniTaskCompletionSource();
        LeanTween.value(gameObject, f => _musicSource.volume = f, _musicSource.volume, 0, fadeOutDuration)
            .setOnComplete(() =>
            {
                _musicSource.Stop();
                musicFadeTsc.TrySetResult();
                registration.Dispose();
            });

        return musicFadeTsc.Task;
    }

    public async UniTask PlayMusicWithFade(CancellationToken stopToken, AudioClip clip, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
    {
        await FadeOutAndStopMusicAsync(stopToken, fadeOutDuration);
        if (stopToken.IsCancellationRequested) return;
        await FadeInAndPlayMusicAsync(stopToken, clip, fadeInDuration);
    }

    private void Activate()
    {
        _settingsModel.AudioMutedStateChanged += OnAudioMutedStateChanged;
        _settingsModel.MusicMutedStateChanged += OnMusicMutedStateChanged;
    }

    private float GetMusicVolume()
    {
        return _settingsModel.IsAudioMuted || _settingsModel.IsMusicMuted ? 0 : 1;
    }

    private void OnAudioMutedStateChanged()
    {
        UpdateVolumes();
    }

    private void OnMusicMutedStateChanged()
    {
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        SetMusicVolume(GetMusicVolume());
        SetSoundsVolume(_settingsModel.IsAudioMuted ? 0 : 1);
    }
}

public class SoundNames
{
    public static string RequestPlace = "button_5";
    public static string Negative1 = "button_2";
    public static string Negative2 = "negative_1";
    public static string Negative3 = "negative_2";
    public static string Button5 = "button_5";
    public static string PopupOpen = "scroll_open_1";
    public static string PopupClose = "scroll_close_1";
    public static string StarsFall = "starsfall";
    public static string NewLevel = "new_level";
    public static string ScoreTick = "score";
    public static string ProductDrop1 = "product_drops_1";
    public static string ProductDrop2 = "product_drops_2";
    public static string ProductDrop3 = "put_product_1";
    public static string ProductPut = "put_product_1";
    public static string Remove = "shopper_take";
    public static string Remove2 = "click_1";
    public static string Rotate = "rotate_object_1";
    public static string Place = "object_placement_1";
    public static string Delivered = "coin_1";
    public static string Clean1 = "cleaning";
    public static string Clean2 = "cleaning_1";
    public static string Clean3 = "cleaning_2";
    public static string Clean4 = "cleaning_3";
    public static string Cash1 = "cashbox_1";
    public static string Cash2 = "cashbox_2";
    public static string MusicBuild1 = "build_theme_1";
    public static string MusicBuild2 = "build_theme_2";
    public static string MusicGameLowMood = "main_theme_1";
    public static string MusicGameHighMood = "main_theme_2";
}
