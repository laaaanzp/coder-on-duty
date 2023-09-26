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
        totalTime = 0f;
        InvokeRepeating("UpdateTimeDisplay", 1f, 1f);
        isStarted = false;
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
        float currentTotalTime = totalTime;
        int totalMinutes = 0;

        if (currentTotalTime >= 60)
        {
            totalMinutes = (int)currentTotalTime / 60;
            currentTotalTime -= totalMinutes * 60;
        }

        int totalSeconds = (int)Math.Round(currentTotalTime);

        string seconds = totalSeconds <= 9 ? $"0{totalSeconds}" : totalSeconds.ToString();
        string minutes = totalMinutes <= 9 ? $"0{totalMinutes}" : totalMinutes.ToString();

        return $"{minutes}:{seconds}";
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
