using UnityEngine;

public class UserManual : MonoBehaviour
{
    public ModalControl computerManualModal;
    public Outline outline;

    private Ticket ticket;


    void Start()
    {
        ticket = TicketManager.RegisterTask(gameObject, "Read User Manual");

        computerManualModal.onClose += delegate
        {
            ticket.Finish();
            outline.OutlineColor = Color.cyan;
        };
    }

    public void Interact()
    {
        computerManualModal.Open();
    }
}
