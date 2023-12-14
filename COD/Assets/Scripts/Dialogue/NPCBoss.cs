using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCBoss : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private ModalControl taskModalControl;
    [SerializeField] private ModalControl manualModalControl;
    [SerializeField] private TicketManager ticketManager;

    [SerializeField] private GameObject scenematicCamera;

    [Header("Camera Positions")]
    [SerializeField] private Transform threeComputersScene;

    private static NPCBoss instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (DatabaseManager.instance.currentLanguage.currentLevel == 1)
        {
            Invoke(nameof(TrainingDialogue1), 1);
        }
        else
        {
            Invoke(nameof(FirstDialogue), 1);
        }
    }

    public void CheckTask()
    {
        if (ticketManager.CheckTasks())
        {
            ticketManager.LevelComplete();
        }
        else
        {
            Say("Have you finished all of your tasks for this day?", ticketManager.taskModalControl.Open);
        }
    }    

    public static void Say(string[] sentences, Action callback = null)
    {
        NPCDialogue dialogue = new NPCDialogue(sentences);
        DialogueManager.StartDialogue(instance.gameObject.name, dialogue, callback);
    }

    public static void Say(string sentence, Action callback = null)
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { sentence });
        DialogueManager.StartDialogue(instance.gameObject.name, dialogue, callback);
    }

    void TrainingDialogue1()
    {
        string[] sentences = {
            "Welcome Trainee. Welcome to the company",
            "Today is your first day and I will be teaching you the basics.",
            "Your job here is to help our employees with their computer problems if they encountered one.",
            "But for now, I will be teaching you the process."
        };
        NPCDialogue dialogue = new NPCDialogue(sentences);

        DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
        {
            scenematicCamera.transform.localPosition = threeComputersScene.localPosition;
            scenematicCamera.transform.localRotation = threeComputersScene.localRotation;
            scenematicCamera.SetActive(true);

            dialogue = new NPCDialogue(new[] { 
                "Located at your back, there are 3 computers with problems. Try to fix all of them.",
                "Each of them has unique mechanics to be fixed. Interact with them by pressing the E button when you hover your cursor over them.",
                "First is drag and drop. You need to drag the correct pieces to the unfilled slots.",
                "Second is drag and arrange. You need to drag the pieces to the other slots to arrange the order of the code.",
                "Lastly, find and arrange. You need to find all the bugs from the code and select them."
            });

            DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
            {
                scenematicCamera.SetActive(false);

                dialogue = new NPCDialogue(new[] {
                    "You have a certain amount of time to fix the computers when you open then. Don't forget to click on the submit button once finished.",
                    "A manual will be your guide on fixing the computers. You can press R to open it.",
                    "Additionally, you can press T to open the remaining task list.",
                    "Once you are finished with all of your tasks, go back to me for your assessment.",
                    "Goodluck!"
                });

                DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
                {
                    PlayerMovement.canMove = true;
                });
            });
        });
    }    

    void FirstDialogue()
    {
        string[] messages = new string[3];
        messages[1] =
            $"Your job for today is to fix the computers of {ElectronicDevice.names[0]}, {ElectronicDevice.names[1]} and {ElectronicDevice.names[2]}.";
        messages[2] = "You can view your tasks by pressing the T key and your user manual by pressing the R key on your keyboard. Goodluck!";

        if (DatabaseManager.instance.currentLanguage.currentLevel < 4)
            messages[0] = "Good day, Trainee.";

        else
            messages[0] = $"Good day, {DatabaseManager.instance.currentLanguage.currentName}.";

        NPCDialogue dialogue = new NPCDialogue(messages);

        DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
        {
            PlayerMovement.canMove = true;
            taskModalControl.Open();
        });
    }
}
