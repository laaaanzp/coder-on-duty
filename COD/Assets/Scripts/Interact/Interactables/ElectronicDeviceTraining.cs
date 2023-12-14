using UnityEngine;

public class ElectronicDeviceTraining : MonoBehaviour
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
        if (puzzleModal == null)
        {
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            interactable.isInteractable = false;
            Destroy(outline);
            return;
        }

        ticket = TicketManager.RegisterTask(gameObject, $"Fix {name}");
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
        interactable.isInteractable = false;

        if (ticket.isFixed)
        {
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            MessageBoxControl.ShowOk("RESULT", $"Device has been fixed.\n\nCorrect Answers: {totalCorrect} out of {totalSlots}\nTime Remaining: {timeRemaining}\nScore: {timeRemaining} * {totalCorrect} = {timeRemaining * totalCorrect}", () =>
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
