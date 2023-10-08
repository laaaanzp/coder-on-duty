using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    public static bool isLevelCompleted;

    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private LevelCompleteModal levelCompleteModalControl;
    [SerializeField] private StatisticsModal statisticsModalControl;
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
        ticketCounterText.text = $"Remaining Active Tickets: {tickets.Count}";
    }

    private static void CheckTasks()
    {
        foreach (Ticket task in tickets)
        {
            if (!task.isFixed)
                return;
        }

        isLevelCompleted = true;

        DatabaseManager.instance.currentLanguage.currentTime += LevelTimer.GetTimeInSeconds();
        DatabaseManager.instance.currentLanguage.currentScore += ScoreManager.score;
        DatabaseManager.instance.currentLanguage.currentTotalAccuracy += ScoreManager.accuracy;

        instance.LevelComplete();
    }

    private void LevelComplete()
    {
        if (DatabaseManager.instance.currentLanguage.isPlayingTheLastLevel)
        {
            int time = DatabaseManager.instance.currentLanguage.currentTime;
            int score = DatabaseManager.instance.currentLanguage.currentScore;

            statisticsModalControl.Open(time, score);
        }
        else
        {
            levelCompleteModalControl.Open();
        }

        DatabaseManager.instance.currentLanguage.LevelUp();
    }
}
