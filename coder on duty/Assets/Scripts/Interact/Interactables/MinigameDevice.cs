using System.Collections.Generic;
using UnityEngine;

public class MinigameDevice : MonoBehaviour
{
    public Material fixedScreenMaterial;
    [HideInInspector] public IPuzzle puzzleModal;

    [SerializeField] private DeviceScreen deviceScreen;
    [SerializeField] private Interactable interactable;
    [SerializeField] private Outline outline;

    private Ticket ticket;
    private bool hasOpened;
    private bool currentlyOpen;

    public void Initialize()
    {
        float delay;
        if (puzzleModal == null)
        {
            delay = Random.Range(0f, 5f);
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            Destroy(outline);
            return;
        }

        delay = Random.Range(0f, 1f);

        interactable.interactionEvent.AddListener(Interact);

        ticket = TicketManager.CreateTask(gameObject, "Minigame");

        puzzleModal.onSubmitOrFinish += OnFinish;
        puzzleModal.SetTicket(ticket);
    }

    public void Interact()
    {
        OpenComputer();
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

                MessageBoxControl.ShowYesNo("INSTRUCTIONS", $"Challenge: <b>{challengeInformation}</b>\n\nTotal Answers: {puzzleModal.totalSlots}\n\nProceed?",
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

        Destroy(outline);
        interactable.isInteractable = false;

        if (ticket.isFixed)
        {
            int additionalScore = 500;
            ScoreManager.AddAdditionalScore(additionalScore);
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            MessageBoxControl.ShowOk("RESULT", $"Minigame Completed. You gained {additionalScore} addition score.", () =>
            {
                puzzleModal.Close();
            });
        }

        else
        {
            MessageBoxControl.ShowOk("RESULT", $"Device is still broken. \n\nCorrect Answers: {totalCorrect} out of {totalSlots}.", () =>
            {
                puzzleModal.Close();
            });
        }
    }
}
