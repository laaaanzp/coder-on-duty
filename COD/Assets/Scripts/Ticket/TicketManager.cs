using System;
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
    public ModalControl taskModalControl;

    private static List<Ticket> tickets;
    public static TicketManager instance;


    void Awake()
    {
        isLevelCompleted = false;
        tickets = new List<Ticket>();
        instance = this;
        taskModalControl.gameObject.SetActive(false);
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

    public static Ticket CreateTask(GameObject owner, string ticketTitle)
    {
        GameObject taskInstance = Instantiate(instance.taskPrefab, instance.transform);

        Ticket task = taskInstance.GetComponent<Ticket>();

        task.Initialize(owner, ticketTitle, null);
        taskInstance.SetActive(false);

        return task;
    }

    public static void OnFinish(Ticket ticket)
    {
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

    public bool CheckTasks()
    {
        foreach (Ticket task in tickets)
        {
            if (!task.isFinished)
            {
                return false;
            }
        }

        return true;
    }

    public void LevelComplete()
    {
        List<TaskScoreModel> taskScoreModels = new List<TaskScoreModel>();

        foreach (IPuzzle problemSolving in ProblemSolvingDistributor.puzzleSolvings)
        {
            taskScoreModels.Add(problemSolving.taskScoreModel);
        }

        isLevelCompleted = true;

        foreach (TaskScoreModel taskScoreModel in taskScoreModels)
        {
            if (taskScoreModel.isFixed)
            {
                Action levelCompleteAction = () => { endScreen.ShowLevelComplete(taskScoreModels.ToArray()); };
                switch (DatabaseManager.instance.currentLanguage.currentLevel)
                {
                    case 1:
                        NPCBoss.Say("Well done on your first day of training. Keep on doing your best!", levelCompleteAction);
                        break;
                    case 2:
                        NPCBoss.Say("Well done on your second day of training. Keep up the great work. Tomorrow will decide if you're going to be promoted to regular. Goodluck!", levelCompleteAction);
                        break;
                    case 3:
                        NPCBoss.Say("Congratulations! You have finished all of your training, you are now promoted to regular!", levelCompleteAction);
                        break;
                    default:
                        NPCBoss.Say("Well done. You have finished all of your tasks for today.", levelCompleteAction);
                        break;
                }

                return;
            }
        }

        NPCBoss.Say("You did your best and that's what matters the most. There is always another chance, better luck next time.", () => { endScreen.ShowGameOver(taskScoreModels.ToArray()); });
    }
}
