using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

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

    public static Ticket RegisterTask(GameObject owner, string ticketTitle)
    {
        GameObject taskInstance = Instantiate(instance.taskPrefab, instance.transform);

        Ticket task = taskInstance.GetComponent<Ticket>();
        tickets.Add(task);

        task.Initialize(owner, ticketTitle, OnFix);
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

        ScoreTracker.AddScore(500);
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
        instance.LevelComplete();

        DatabaseManager.instance.currentLanguage.UpdateCurrentTime(LevelTimer.GetTimeInSeconds());
        DatabaseManager.instance.currentLanguage.UpdateCurrentScore(ScoreTracker.score);
        DatabaseManager.instance.currentLanguage.LevelUp();
    }

    private void LevelComplete()
    {
        levelCompleteModalControl.Open();
    }
}
