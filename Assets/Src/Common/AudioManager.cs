using System;
using UnityEngine;

public class AudioManager
{
    public static AudioManager Instance => _instance.Value;
    private static Lazy<AudioManager> _instance = new Lazy<AudioManager>();

    public AudioClip[] Sounds { get; private set; }

    private AudioSource _soundsSource;

    public void SetSounds(AudioClip[] sounds)
    {
        Sounds = sounds;
    }

    public void PlaySound(string soundName)
    {
        foreach (var sound in Sounds)
        {
            if (sound.name == soundName)
            {
                PlaySound(sound);
            }
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (_soundsSource == null)
        {
            _soundsSource = Camera.main.GetComponent<AudioSource>();
        }
        _soundsSource.PlayOneShot(sound);
    }
}

public class SoundNames
{
    public static string PopupOpen = "scroll_open_1";
    public static string PopupClose = "scroll_close_1";
    public static string StarsFall = "starsfall";
    public static string NewLevel = "new_level";
    public static string ScoreTick = "score";
}
