using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TicketManager : MonoBehaviour
{
    public static bool isLevelCompleted;

    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private LevelCompleteModal levelCompleteModalControl;
    [SerializeField] private GameOverModal gameOverModalControl;
    [SerializeField] private TextMeshProUGUI ticketCounterText;

    private static List<Ticket> tickets;
    private static TicketManager instance;


    void Awake()
    {
        isLevelCompleted = false;
        tickets = new List<Ticket>();
        instance = this;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Introduction"))
        {
            TutorialModalControl.Open();
        }
    }

    public static Ticket RegisterTask(GameObject owner, string ticketTitle)
    {
        GameObject taskInstance = Instantiate(instance.taskPrefab, instance.transform);

        Ticket task = taskInstance.GetComponent<Ticket>();
        tickets.Add(task);

        task.Initialize(owner, ticketTitle, OnFinish);
        instance.UpdateRemainingActiveTicketsDisplay();

        return task;
    }

    public static void OnFinish(Ticket ticket)
    {
        instance.CheckTasks();
        tickets.Remove(ticket);
        instance.UpdateRemainingActiveTicketsDisplay();

        if (ObjectNavigation.instance.currentInstance == ticket.owner)
        {
            ObjectNavigation.StopNavigate();
        }
    }

    private void UpdateRemainingActiveTicketsDisplay()
    {
        ticketCounterText.text = $"Remaining Tasks: {tickets.Count}";
    }

    private void CheckTasks()
    {
        foreach (Ticket task in tickets)
        {
            if (!task.isFinished)
            {
                return;
            }
        }

        if (ScoreManager.fixedDevices == 0)
        {
            gameOverModalControl.Open();
            return;
        }

        isLevelCompleted = true;

        DatabaseManager.instance.currentLanguage.currentTime += LevelTimer.GetTimeInSeconds();
        DatabaseManager.instance.currentLanguage.currentScore += ScoreManager.score;
        DatabaseManager.instance.currentLanguage.currentTotalAccuracy += ScoreManager.accuracy;
        DatabaseManager.instance.currentLanguage.currentTotalStars += ScoreManager.fixedDevices;
        DatabaseManager.instance.currentLanguage.SetLevelDataByName(SceneManager.GetActiveScene().name, ScoreManager.score, LevelTimer.GetTimeInSeconds(), ScoreManager.accuracy, ScoreManager.fixedDevices);

        LevelComplete();
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
