using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class Clock : MonoBehaviour
{
    public bool is24HourFormat;

    [Header("Clock Hands")]
    public Transform secondsHand;
    public Transform minuteHand;
    public Transform hourHand;

    [Header("Clock Audio")]
    public AudioClip[] tickSoundEffects;
    public AudioSource tickPlayer;


    private void Awake()
    {
        // Update clock every second
        InvokeRepeating("UpdateClock", 1f, 1f);
    }

    private void UpdateClock()
    {

        DateTime currentTime = DateTime.Now;

        // 60 seconds/minutes * 6 = 360
        float secondsHandRotation = currentTime.Second * 6;
        float minuteHandRotation = currentTime.Minute * 6;

        // Fix for 12-hour format
        int currentHourTime = currentTime.Hour;
        if (!is24HourFormat && currentHourTime > 12)
            currentHourTime -= 12;

                                                          // Offset that depends on the minute
                                                          // to make it more accurate
        float hourHandRotation = (currentHourTime * 30) + ((float)currentTime.Minute / 60 * 30);

        if (secondsHand.eulerAngles.z != secondsHandRotation)
        {
            PlayRandomTickSound();
        }
        
        secondsHand.eulerAngles = 
            new Vector3(
                secondsHand.eulerAngles.x,
                secondsHand.eulerAngles.y,
                secondsHandRotation
                );

        minuteHand.eulerAngles =
            new Vector3(
                minuteHand.eulerAngles.x,
                minuteHand.eulerAngles.y,
                minuteHandRotation
                );

        hourHand.eulerAngles =
            new Vector3(
                hourHand.eulerAngles.x,
                hourHand.eulerAngles.y,
                hourHandRotation
                );
    }

    private void PlayRandomTickSound()
    {
        int randomIndex = Random.Range(0, tickSoundEffects.Length);
        AudioClip randomTickSoundEffect = tickSoundEffects[randomIndex];

        tickPlayer.PlayOneShot(randomTickSoundEffect);
    }
}
