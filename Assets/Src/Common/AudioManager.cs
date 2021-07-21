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

    public void PlayRandomSound(params string[] soundNames)
    {
        var index = UnityEngine.Random.Range(0, soundNames.Length - 1);
        PlaySound(soundNames[index]);
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
}
