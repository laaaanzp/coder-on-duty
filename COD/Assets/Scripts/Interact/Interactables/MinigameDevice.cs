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
                string challengeType;

                if (puzzleModal is DragAndArrangePuzzle)
                    challengeType = "Drag and Arrange";

                else if (puzzleModal is DragAndDropPuzzle)
                    challengeType = "Drag and Drop";

                else if (puzzleModal is Crossword)
                    challengeType = "Crossword";

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

        Destroy(outline);

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
