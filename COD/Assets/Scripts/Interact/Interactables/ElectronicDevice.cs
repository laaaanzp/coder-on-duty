using System.Collections.Generic;
using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{
    public Material fixedScreenMaterial;
    [HideInInspector] public IPuzzle puzzleModal;

    [SerializeField] private DeviceScreen deviceScreen;
    [SerializeField] private Interactable interactable;
    [SerializeField] private Outline outline;
    [SerializeField] private Animator animator;

    private Ticket ticket;
    private bool hasOpened;
    private bool currentlyOpen;

    public static List<string> names;

    private string[] greetings = { "Hello!", "Hello there!", "Hi!", "Hi there!", "Hey", "Hey there!" };
    private string[] messages = { "How are you?", "How's it going?", "What's up?", "How do you do?", "I'm kind of busy right now." };
    private string[] problemMessages = 
    {
        "Can you help me with my computer? It's not working well.",
        "My computer has stopped working. Can you fix it for me?",
        "My computer suddenly stopped working. Can you help me?",
        "My computer keeps on crashing. Can you please help me?",
        "Can you help me? My computer encountered an error.",
        "I need my computer today and it's not working properly. Help me please!"
    };
    private string[] randomThankYouMessages =
    {
        "Thank you for helping me.",
        "Thank you for fixing my computer.",
        "Thank you so much for fixing my computer.",
        "My computer is now working fine. Thanks to you.",
        "My computer is working again. Thank you.",
    };
    private string[] randomThankYouMessages2 =
    {
        "Thank you for trying to fix my computer.",
        "At least you tried. Thank you.",
        "I know you tried. Thank you.",
        "It's unfortunate but you tried your best so it's fine. Thank you."
    };

    private string randomGreeting;
    private string randomMessage;
    private string randomProblemMessage;
    private string randomThankYouMessage;
    private string randomThankYouMessage2;


    void Awake()
    {
        names = new List<string>();

        randomGreeting = greetings[Random.Range(0, greetings.Length)];
        randomMessage = messages[Random.Range(0, messages.Length)];
        randomProblemMessage = problemMessages[Random.Range(0, problemMessages.Length)];
        randomThankYouMessage = randomThankYouMessages[Random.Range(0, randomThankYouMessages.Length)];
        randomThankYouMessage2 = randomThankYouMessages2[Random.Range(0, randomThankYouMessages2.Length)];
    }

    private void SetTyping()
    {
        animator?.SetBool("Typing", true);
    }    

    private void SetAngry()
    {
        animator?.SetBool("Typing", false);
    }

    public void Initialize()
    {
        float delay;
        if (puzzleModal == null)
        {
            delay = Random.Range(0f, 5f);
            Invoke(nameof(SetTyping), delay);
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            Destroy(outline);
            interactable.interactionEvent.AddListener(InteractDefault);
            return;
        }

        delay = Random.Range(0f, 2f);
        Invoke(nameof(SetAngry), delay);

        names.Add(name);

        interactable.interactionEvent.AddListener(Interact);

        ticket = TicketManager.RegisterTask(gameObject, $"Fix {name}'s computer");
        puzzleModal.onSubmitOrFinish += OnFinish;
        puzzleModal.SetTicket(ticket);
    }

    private void InteractDefault()
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { randomGreeting, randomMessage });

        DialogueManager.StartDialogue(name, dialogue);
    }

    private void InteractPositive()
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { randomThankYouMessage });
        DialogueManager.StartDialogue(name, dialogue);
    }

    private void InteractNegative()
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { randomThankYouMessage2 });
        DialogueManager.StartDialogue(name, dialogue);
    }

    private void Interact()
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { randomGreeting, randomProblemMessage });
        DialogueManager.StartDialogue(name, dialogue, OpenComputer);
    }

    private void OpenComputer()
    {
        if (!hasOpened)
        {
            if (!currentlyOpen)
            {
                string challengeType;

                if (puzzleModal is DragAndArrangePuzzle)
                    challengeType = "Drag and Arrange";

                else if (puzzleModal is DragAndDropPuzzle)
                    challengeType = "Drag and Drop";

                else
                    challengeType = "Find and Select bugs";

                MessageBoxControl.ShowYesNo("INSTRUCTIONS", $"Challenge: <b>{challengeType}</b>\nTotal Answers: {puzzleModal.totalSlots}\n\nYou have 180 seconds to solve the problem. Do you want to proceed?",
                    () =>
                    {
                        puzzleModal.Open();
                        hasOpened = true;
                        currentlyOpen = false;
                    },
                    () =>
                    {
                        currentlyOpen = false;
                    });

                currentlyOpen = true;
            }
        }
        else
        {
            puzzleModal.Open();
        }
    }

    public void OnFinish()
    {
        int totalCorrect = puzzleModal.totalCorrect;
        int timeRemaining = puzzleModal.timeRemaining;
        int totalSlots = puzzleModal.totalSlots;

        TicketManager.OnFinish(ticket);
        
        Destroy(outline);

        if (ticket.isFixed)
        {
            SetTyping();
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            MessageBoxControl.ShowOk("RESULT", $"Device has been fixed.\n\nCorrect Answers: {totalCorrect} out of {totalSlots}\nTime Remaining: {timeRemaining}\nScore: {timeRemaining} * {totalCorrect} = {timeRemaining * totalCorrect}", () =>
            {
                puzzleModal.Close();
                interactable.interactionEvent.RemoveAllListeners();
                interactable.interactionEvent.AddListener(InteractPositive);
            });
        }

        else
        {
            MessageBoxControl.ShowOk("RESULT", $"Device is still broken. \n\nCorrect Answers: {totalCorrect} out of {totalSlots}.", () =>
            {
                puzzleModal.Close();
                interactable.interactionEvent.RemoveAllListeners();
                interactable.interactionEvent.AddListener(InteractNegative);
            });
        }
    }
}
