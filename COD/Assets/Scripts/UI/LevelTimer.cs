using System;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static int totalMinutes;
    public static int totalSeconds;
    public static bool isStarted;

    [SerializeField] private TextMeshProUGUI timeText;

    private static float totalTime;
    

    void Awake()
    {
        totalMinutes = totalSeconds = 0;
        InvokeRepeating("UpdateTimeDisplay", 1f, 1f);
        isStarted = false;
    }

    void Start()
    {
        totalTime = DatabaseManager.instance.currentLanguage.currentTime;
    }

    public void StartTimer()
    {
        isStarted = true;
    }

    public static int GetTimeInSeconds()
    {
        return (int)Math.Round(totalTime);
    }

    public static string GetTimeAsString()
    {
        int totalSeconds = (int)totalTime;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        return timeString;
    }

    void UpdateTimeDisplay()
    {
        timeText.text = GetTimeAsString();
    }

    void Update()
    {
        if (TicketManager.isLevelCompleted || !isStarted) 
            return;

        totalTime += Time.deltaTime;
    }
}
