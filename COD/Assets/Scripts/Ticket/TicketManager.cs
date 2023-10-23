using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TicketManager : MonoBehaviour
{
    public static bool isLevelCompleted;

    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private EndScreen endScreen;
    [SerializeField] private TextMeshProUGUI ticketCounterText;

    private static List<Ticket> tickets;
    public static TicketManager instance;


    void Awake()
    {
        isLevelCompleted = false;
        tickets = new List<Ticket>();
        instance = this;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Introduction")
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
        // instance.CheckTasks();
        tickets.Remove(ticket);
        Destroy(ticket.gameObject);
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

    public void CheckTasks()
    {
        foreach (Ticket task in tickets)
        {
            if (!task.isFinished)
            {
                return;
            }
        }

        List<TaskScoreModel> taskScoreModels = new List<TaskScoreModel>();

        foreach (ProblemSolving problemSolving in ProblemSolvingDistributor.problemSolvings)
        {
            taskScoreModels.Add(problemSolving.taskScoreModel);
        }

        isLevelCompleted = true;
        LevelComplete(taskScoreModels);
    }

    private void LevelComplete(List<TaskScoreModel> taskScoreModels)
    {
        foreach (TaskScoreModel taskScoreModel in taskScoreModels)
        {
            if (taskScoreModel.isFixed)
            {
                endScreen.ShowLevelComplete(taskScoreModels.ToArray());
                return;
            }
        }

        endScreen.ShowGameOver(taskScoreModels.ToArray());
    }
}
