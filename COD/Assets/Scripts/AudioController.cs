using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource menuBGMAudioSource;
    [SerializeField] private AudioClip nodeSlotCorrect;
    [SerializeField] private AudioClip nodeSlotIncorrect;
    [SerializeField] private AudioClip fixedDevice;
    [SerializeField] private AudioClip unfixedDevice;
    [SerializeField] private AudioClip footstepLeft;
    [SerializeField] private AudioClip footstepRight;
    private bool isLeftFootstep;
    [SerializeField] private AudioClip doorInteract;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;

    void Awake()
    {
        instance = this;
        float musicVolume = PlayerPrefs.GetFloat("music-volume", 1f);
        musicAudioSource.volume = musicVolume;
        menuBGMAudioSource.volume = musicVolume * 0.65f;
        sfxAudioSource.volume = PlayerPrefs.GetFloat("sfx-volume", 1f);
    }

    public static void SetSfxVolume(float volume)
    {
        instance.sfxAudioSource.volume = volume;
    }

    public static void SetMusicVolume(float volume)
    {
        instance.musicAudioSource.volume = volume;
        instance.menuBGMAudioSource.volume = volume * 0.65f;
    }

    public static void PlayNodeSlotCorrect()
    {
        instance.sfxAudioSource.PlayOneShot(instance.nodeSlotCorrect);
    }

    public static void PlayNodeSlotIncorrect()
    {
        instance.sfxAudioSource.PlayOneShot(instance.nodeSlotIncorrect);
    }

    public static void PlayFixedDevice()
    {
        instance.sfxAudioSource.PlayOneShot(instance.fixedDevice);
    }

    public static void PlayUnfixedDevice()
    {
        instance.sfxAudioSource.PlayOneShot(instance.unfixedDevice);
    }

    public static void PlayFootstep()
    {
        instance.sfxAudioSource.PlayOneShot(instance.isLeftFootstep ? instance.footstepLeft : instance.footstepRight);
        instance.isLeftFootstep = !instance.isLeftFootstep;
    }

    public static void PlayDoorInteract()
    {
        instance.sfxAudioSource.PlayOneShot(instance.doorInteract);
    }

    public static void PlayBeep()
    {
        instance.sfxAudioSource.PlayOneShot(instance.beepSound);
    }

    public static void PlayTyping()
    {
        instance.sfxAudioSource.PlayOneShot(instance.typingSound);
    }

    public static void PlayButtonHover()
    {
        instance.sfxAudioSource.PlayOneShot(instance.buttonHoverSound, 0.25f);
    }

    internal static void PlayButtonClick()
    {
        instance.sfxAudioSource.PlayOneShot(instance.buttonClickSound, 0.5f);
    }
}
