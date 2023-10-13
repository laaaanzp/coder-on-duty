using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{
    public Material fixedScreenMaterial;
    [HideInInspector] public ProblemSolving problemModal;

    [SerializeField] private DeviceScreen deviceScreen;
    [SerializeField] private Interactable interactable;
    [SerializeField] private Outline outline;


    private Ticket ticket;
    private bool hasOpened;


    public void Initialize()
    {
        if (problemModal == null)
        {
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            Destroy(outline);
            Destroy(interactable);
            return;
        }
        ticket = TicketManager.RegisterTask(gameObject, $"Fix Device ({name})");
        problemModal.onSubmitOrFinish += () => 
        { 
            OnFinish();
        };
        problemModal.SetTicket(ticket);
        interactable.interactionEvent.AddListener(Interact);
    }

    bool currentlyOpen;
    public void Interact()
    {
        if (!hasOpened)
        {
            if (!currentlyOpen)
            {
                MessageBoxControl.ShowYesNo("INSTRUCTIONS", "Fill the slots with the correct nodes.\n\nYou have 120 seconds to solve the problem. Do you want to proceed?",
                    () =>
                    {
                        problemModal.Open();
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
            problemModal.Open();
        }
    }

    public void OnFinish()
    {
        int totalCorrect = problemModal.totalCorrect;
        int timeRemaining = problemModal.timeRemaining;

        if (ticket.isFixed)
        {
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            if (ScoreManager.finishedDevices != 3)
            {
                MessageBoxControl.ShowOk("RESULT", $"Device has been fixed.\n\nCorrect Answers: {totalCorrect} out of 5\nTime Remaining: {timeRemaining}\nAdditional Score: {timeRemaining*10}");
            }
        }

        else
        {
            if (ScoreManager.finishedDevices != 3)
            {
                MessageBoxControl.ShowOk("RESULT", $"Device is still broken. \n\nYou scored {totalCorrect} out of 5.");
            }
        }

        TicketManager.OnFinish(ticket);
        interactable.isInteractable = false;
        Destroy(outline);

    }
}
