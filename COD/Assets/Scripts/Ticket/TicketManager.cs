using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TicketManager : MonoBehaviour
{
    public static bool isLevelCompleted;

    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private LevelCompleteModal levelCompleteModalControl;
    [SerializeField] private TextMeshProUGUI ticketCounterText;

    private static List<Ticket> tickets;
    private static TicketManager instance;


    void Awake()
    {
        isLevelCompleted = false;
        tickets = new List<Ticket>();
        instance = this;
    }

    public static Ticket RegisterTask(GameObject owner, string ticketTitle, int score)
    {
        GameObject taskInstance = Instantiate(instance.taskPrefab, instance.transform);

        Ticket task = taskInstance.GetComponent<Ticket>();
        tickets.Add(task);

        task.Initialize(owner, ticketTitle, score, OnFix);
        instance.UpdateRemainingActiveTicketsDisplay();

        return task;
    }

    private static void OnFix(Ticket ticket)
    {
        tickets.Remove(ticket);
        instance.UpdateRemainingActiveTicketsDisplay();

        if (ObjectNavigation.instance.currentInstance == ticket.owner)
        {
            ObjectNavigation.StopNavigate();
        }

        ScoreManager.AddScore(ticket.score);
        CheckTasks();
    }

    private void UpdateRemainingActiveTicketsDisplay()
    {
        ticketCounterText.text = $"Remaining Tasks: {tickets.Count}";
    }

    private static void CheckTasks()
    {
        foreach (Ticket task in tickets)
        {
            if (!task.isFixed)
                return;
        }

        isLevelCompleted = true;

        int stars = 1;
        if (LevelTimer.onTime)
        {
            stars++;
        }

        if (ScoreManager.accuracy == 100f)
        {
            stars++;
        }

        DatabaseManager.instance.currentLanguage.currentTime += LevelTimer.GetTimeInSeconds();
        DatabaseManager.instance.currentLanguage.currentScore += ScoreManager.score;
        DatabaseManager.instance.currentLanguage.currentTotalAccuracy += ScoreManager.accuracy;
        DatabaseManager.instance.currentLanguage.currentTotalStars += stars;

        instance.LevelComplete();
    }

    private void LevelComplete()
    {
        LanguageDatabase languageDatabase = DatabaseManager.instance.currentLanguage;

        int time = languageDatabase.currentTime;
        int score = languageDatabase.currentScore;
        float accuracy = languageDatabase.overallAccuracy;
        float stars = languageDatabase.overallStars;

        levelCompleteModalControl.Open(time, score, accuracy, stars, languageDatabase.isPlayingTheLastLevel);

        DatabaseManager.instance.currentLanguage.LevelUp();
    }
}
