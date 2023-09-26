using UnityEngine;

public class TicketModalControl : MonoBehaviour
{
    [SerializeField] private ModalControl ticketModalControl;


    void Update()
    {
        if (ModalObserver.activeModalCount > 0)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ticketModalControl.Open();
        }
    }
}
