using System;
using System.Collections;
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

        randomGreeting = greetings[UnityEngine.Random.Range(0, greetings.Length)];
        randomMessage = messages[UnityEngine.Random.Range(0, messages.Length)];
        randomProblemMessage = problemMessages[UnityEngine.Random.Range(0, problemMessages.Length)];
        randomThankYouMessage = randomThankYouMessages[UnityEngine.Random.Range(0, randomThankYouMessages.Length)];
        randomThankYouMessage2 = randomThankYouMessages2[UnityEngine.Random.Range(0, randomThankYouMessages2.Length)];
    }

    public void SetTyping()
    {
        animator?.SetBool("Typing", true);
    }    

    public void SetAngry()
    {
        animator?.SetBool("Typing", false);
    }

    IEnumerator InvokeAsync(Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }

    public void Initialize()
    {
        float delay;
        if (puzzleModal == null)
        {
            delay = UnityEngine.Random.Range(0.9f, 1.5f);
            animator.speed = delay;
            StartCoroutine(InvokeAsync(SetTyping, delay));
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            Destroy(outline);
            interactable.interactionEvent.AddListener(InteractDefault);
            return;
        }

        delay = UnityEngine.Random.Range(0, 2f);
        StartCoroutine(InvokeAsync(SetAngry, delay));

        names.Add(name);

        interactable.interactionEvent.AddListener(Interact);

        ticket = TicketManager.RegisterTask(gameObject, $"Fix {name}'s computer");
        puzzleModal.onSubmitOrFinish += OnFinish;
        puzzleModal.SetTicket(ticket);

        CheckIfFinished();
    }

    private void InteractDefault()
    {
        NPCDialogue dialogue = new NPCDialogue(new string[] { randomGreeting, randomMessage });
        DialogueManager.StartDialogue(name, dialogue);
    }

    private void InteractPositive()
    {
        if (DatabaseManager.instance.currentLanguage.currentLevel == 2)
        {
            switch (gameObject.name)
            {
                case "Naym":
                    randomThankYouMessage = "Oh, it was a syntax error that's causing me the troubles. Thank you for your help.";
                    break;
                case "Matthew":
                    randomThankYouMessage = "Wow! You managed to fix and open my code editor. I've been trying to do that for an hour now. Thank you!";
                    break;
                case "Arian":
                    randomThankYouMessage = "Thank You! My computer now runs smoother. I just hope none of my applications crashes again.";
                    break;
            }
        }

        NPCDialogue dialogue = new NPCDialogue(new string[] { randomThankYouMessage });
        DialogueManager.StartDialogue(name, dialogue);
    }

    private void InteractNegative()
    {
        if (DatabaseManager.instance.currentLanguage.currentLevel == 2)
        {
            switch (gameObject.name)
            {
                case "Naym":
                    randomThankYouMessage2 = "I'll just recode everything. Thank you for helping!";
                    break;
                case "Matthew":
                    randomThankYouMessage2 = "I guess I need to change my code editor now. Thank you for trying to help me.";
                    break;
                case "Arian":
                    randomThankYouMessage2 = "I'll just request for a new computer. Thanks for the help though.";
                    break;
            }
        }

        NPCDialogue dialogue = new NPCDialogue(new string[] { randomThankYouMessage2 });
        DialogueManager.StartDialogue(name, dialogue);
    }

    private void Interact()
    {
        if (DatabaseManager.instance.currentLanguage.currentLevel == 2)
        {
            switch (gameObject.name)
            {
                case "Naym":
                    randomGreeting = "Hi there. You're the new trainee right?";
                    randomProblemMessage = "I need your assistance. My IDE has been throwing a lot of errors suddenly and I couldn't find the cause of it. Can you help me?";
                    break;
                case "Matthew":
                    randomGreeting = "Oh hi! It's the new trainee.";
                    randomProblemMessage = "Quick! I need help. I am unable to do anything for an hour now because I am unable to open my code editor.";
                    break;
                case "Arian":
                    randomGreeting = "Hi, new trainee.";
                    randomProblemMessage = "Will you be able to help me with my computer? It became slower suddenly and it sometimes crashes my applications.";
                    break;
            }
        }
        else if (DatabaseManager.instance.currentLanguage.currentLevel == 3)
        {
            switch (gameObject.name)
            {
                case "Naym":
                    randomGreeting = "Hi there. You're the new trainee right?";
                    randomProblemMessage = "I need your assistance. My IDE has been throwing a lot of errors suddenly and I couldn't find the cause of it. Can you help me?";
                    break;
                case "Matthew":
                    randomGreeting = "Oh hi! It's the new trainee.";
                    randomProblemMessage = "Quick! I need help. I am unable to do anything for an hour now because I am unable to open my code editor.";
                    break;
                case "Arian":
                    randomGreeting = "Hi, new trainee.";
                    randomProblemMessage = "Will you be able to help me with my computer? It became slower suddenly and it sometimes crashes my applications.";
                    break;
            }
        }


        NPCDialogue dialogue = new NPCDialogue(new string[] { randomGreeting, randomProblemMessage });
        DialogueManager.StartDialogue(name, dialogue, OpenComputer);
    }

    private void CheckIfFinished()
    {
        return;
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string type = puzzleModal.GetType().ToString();

        if (PlayerPrefs.GetInt($"{languageName}-{type}-total-correct", -1) <= -1)
        {
            return;
        }

        puzzleModal.totalCorrect = PlayerPrefs.GetInt($"{languageName}-{type}-total-correct");
        puzzleModal.timeRemaining = PlayerPrefs.GetInt($"{languageName}-{type}-time-remaining");
        puzzleModal.totalSlots = PlayerPrefs.GetInt($"{languageName}-{type}-total-slots");

        TicketManager.OnFinish(ticket);

        Destroy(outline);

        ticket.isFixed = puzzleModal.totalCorrect > (float)puzzleModal.totalSlots / 2;

        if (ticket.isFixed)
        {
            SetTyping();
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            interactable.interactionEvent.RemoveAllListeners();
            interactable.interactionEvent.AddListener(InteractPositive);

        }
        else
        {
            interactable.interactionEvent.RemoveAllListeners();
            interactable.interactionEvent.AddListener(InteractNegative);
        }
    }

    private void OpenComputer()
    {
        if (!hasOpened)
        {
            if (!currentlyOpen)
            {
                string challengeInformation;

                if (puzzleModal is DragAndArrangePuzzle)
                    challengeInformation = "Drag and Arrange</b>\n\nDrag and arrange the pieces to their correct order.";

                else if (puzzleModal is DragAndDropPuzzle)
                    challengeInformation = "Drag and Drop</b>\n\nDrag the correct pieces to the slot.";

                else if (puzzleModal is Crossword)
                    challengeInformation = "Crossword</b>\n\nFind answers inside the crossword and type it inside the input field.";

                else if (puzzleModal is WordSearch)
                    challengeInformation = "Word Search</b>\n\nFind and mark all the words hidden inside the box.";

                else if (puzzleModal is MatchingType)
                    challengeInformation = "Matching Type</b>\n\nMatch the left with the corresponding definition in the right side.";

                else
                    challengeInformation = "Find and Select bugs</b>\n\nLocate and select all the bugs inside the code.";


                MessageBoxControl.ShowYesNo("INSTRUCTIONS", $"Challenge: <b>{challengeInformation}</b>\nTotal Answers: {puzzleModal.totalSlots}\n\nYou have 180 seconds to solve the problem. Do you want to proceed?",
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

        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string type = puzzleModal.GetType().ToString();


        PlayerPrefs.SetInt($"{languageName}-{type}-total-correct", totalCorrect);
        PlayerPrefs.SetInt($"{languageName}-{type}-time-remaining", timeRemaining);
        PlayerPrefs.SetInt($"{languageName}-{type}-total-slots", totalSlots);
        PlayerPrefs.Save();

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
