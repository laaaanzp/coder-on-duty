using UnityEngine;

public class TicketModalControl : MonoBehaviour
{
    [SerializeField] private ModalControl ticketModalControl;
    public static TicketModalControl instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (ModalObserver.activeModalCount > 0)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ticketModalControl.Open();
        }
    }

    public static void Close()
    {
        instance.ticketModalControl.Close();
    }
}
