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
    [SerializeField] private Transform developersScene;
    [SerializeField] private Transform aScene;
    [SerializeField] private Transform bScene;
    [SerializeField] private Transform minigameScene;

    private static NPCBoss instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int currentLevel = DatabaseManager.instance.currentLanguage.currentLevel;

        switch (currentLevel)
        {
            case 1:
                Invoke(nameof(TrainingDialogue1), 1);
                break;
            case 2:
                Invoke(nameof(TrainingDialogue2), 1);
                break;
            case 3:
                Invoke(nameof(TrainingDialogue3), 1);
                break;
            default:
                Invoke(nameof(FirstDialogue), 1);
                break;
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
            LeanTween.move(scenematicCamera, scenematicCamera.transform.position - scenematicCamera.transform.TransformDirection(Vector3.forward * 0.3f), 10.0f).setIgnoreTimeScale(true);

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

    void TrainingDialogue2()
    {
        string[] sentences = {
            "Welcome on your 2nd day of training, Trainee.",
        };
        NPCDialogue dialogue = new NPCDialogue(sentences);

        DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
        {
            scenematicCamera.transform.localPosition = developersScene.localPosition;
            scenematicCamera.transform.localRotation = developersScene.localRotation;
            scenematicCamera.SetActive(true);
            LeanTween.move(scenematicCamera, scenematicCamera.transform.position - scenematicCamera.transform.TransformDirection(Vector3.forward * 0.3f), 10.0f).setIgnoreTimeScale(true);

            dialogue = new NPCDialogue(new[] {
                "Today, 3 of our developers encountered a problem with their computers.",
                "As a part of your test, try to fix their computers.",
                "Remember to use your manual if you need a guide."
            });

            DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
            {
                LeanTween.cancel(scenematicCamera);
                scenematicCamera.transform.localPosition = aScene.localPosition;
                scenematicCamera.transform.localRotation = aScene.localRotation;
                LeanTween.move(scenematicCamera, scenematicCamera.transform.position - scenematicCamera.transform.TransformDirection(Vector3.forward * 0.3f), 10.0f).setIgnoreTimeScale(true);

                dialogue = new NPCDialogue(new[] {
                    "If you managed to pass the todays training. You will be able to access the bigger rooms."
                });

                DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
                {
                    scenematicCamera.SetActive(false);
                    dialogue = new NPCDialogue(new[] {
                        "Come back to me once you are done.",
                        "Goodluck!"
                    });

                    DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
                    {
                        PlayerMovement.canMove = true;
                    });
                });
            });
        });
    }

    void TrainingDialogue3()
    {
        string[] sentences = {
            "Welcome on your last day of training, Trainee.",
            "Todays training will decide your promotion."
        };
        NPCDialogue dialogue = new NPCDialogue(sentences);

        DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
        {
            scenematicCamera.transform.localPosition = aScene.localPosition;
            scenematicCamera.transform.localRotation = aScene.localRotation;
            scenematicCamera.SetActive(true);
            LeanTween.move(scenematicCamera, scenematicCamera.transform.position - scenematicCamera.transform.TransformDirection(Vector3.forward * 0.3f), 10.0f).setIgnoreTimeScale(true);

            dialogue = new NPCDialogue(new[] {
                "Since now you have access to the bigger rooms, help someone who needs your help from there.",
                "Navigate and find them and try to help them."
            });

            DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
            {
                LeanTween.cancel(scenematicCamera);
                scenematicCamera.transform.localPosition = minigameScene.localPosition;
                scenematicCamera.transform.localRotation = minigameScene.localRotation;
                LeanTween.move(scenematicCamera, scenematicCamera.transform.position - scenematicCamera.transform.TransformDirection(Vector3.forward * 0.3f), 10.0f).setIgnoreTimeScale(true);

                dialogue = new NPCDialogue(new[] {
                    "If you managed to pass your final training, you will be able to access the special room.",
                    "With the special room, you will be able to access a computer where you can learn additional knowledge by trying to fix that computer."
                });

                DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
                {
                    scenematicCamera.SetActive(false);
                    dialogue = new NPCDialogue(new[] {
                        "After that, come back here and we will decide for your promotion.",
                        "Goodluck!"
                    });

                    DialogueManager.StartDialogue(gameObject.name, dialogue, () =>
                    {
                        PlayerMovement.canMove = true;
                    });
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
