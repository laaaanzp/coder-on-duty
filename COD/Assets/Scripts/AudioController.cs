using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    [SerializeField] private AudioSource audioSource;

    [Header("Problem Solving")]
    [SerializeField] private AudioClip nodeSlotCorrect;
    [SerializeField] private AudioClip nodeSlotIncorrect;

    [Header("Result")]
    [SerializeField] private AudioClip fixedDevice;
    [SerializeField] private AudioClip unfixedDevice;

    [Header("Footstep")]
    [SerializeField] private AudioClip footstepLeft;
    [SerializeField] private AudioClip footstepRight;
    private bool isLeftFootstep;

    [Header("Door")]
    [SerializeField] private AudioClip doorInteract;

    [Header("Door")]
    [SerializeField] private AudioClip beepSound;

    void Awake()
    {
        instance = this;
        audioSource.volume = PlayerPrefs.GetFloat("music-volume", 1f);
    }

    public static void SetVolume(float volume)
    {
        instance.audioSource.volume = volume;
    }

    public static void PlayNodeSlotCorrect()
    {
        instance.audioSource.PlayOneShot(instance.nodeSlotCorrect);
    }

    public static void PlayNodeSlotIncorrect()
    {
        instance.audioSource.PlayOneShot(instance.nodeSlotIncorrect);
    }

    public static void PlayFixedDevice()
    {
        instance.audioSource.PlayOneShot(instance.fixedDevice);
    }

    public static void PlayUnfixedDevice()
    {
        instance.audioSource.PlayOneShot(instance.unfixedDevice);
    }

    public static void PlayFootstep()
    {
        instance.audioSource.PlayOneShot(instance.isLeftFootstep ? instance.footstepLeft : instance.footstepRight);
        instance.isLeftFootstep = !instance.isLeftFootstep;
    }

    public static void PlayDoorInteract()
    {
        instance.audioSource.PlayOneShot(instance.doorInteract);
    }

    public static void PlayBeep()
    {
        instance.audioSource.PlayOneShot(instance.beepSound);
    }
}
