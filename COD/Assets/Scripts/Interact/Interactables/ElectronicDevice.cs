using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{
    public Material fixedScreenMaterial;
    [HideInInspector] public ProblemSolving problemModal;

    [SerializeField] private DeviceScreen deviceScreen;
    [SerializeField] private Interactable interactable;
    [SerializeField] private Outline outline;

    private Ticket ticket;
    private bool isFixed;


    public void Initialize()
    {
        if (problemModal == null)
        {
            deviceScreen.ChangeScreen(fixedScreenMaterial);
            Destroy(outline);
            return;
        }

        ticket = TicketManager.RegisterTask(gameObject, $"Fix Device ({name})");
        problemModal.onFix += OnFix;

        interactable.interactionEvent.AddListener(Interact);
    }

    public void Interact()
    {
        problemModal.Open();
    }

    private void OnFix()
    {
        if (isFixed)
            return;

        deviceScreen.ChangeScreen(fixedScreenMaterial);
        Destroy(outline);

        interactable.isInteractable = false;

        ticket.Finish();
        isFixed = true;
    }
}
