using UnityEngine;

public class UserManual : MonoBehaviour
{
    public ModalControl computerManualModal;
    public Outline outline;

    public void Interact()
    {
        computerManualModal.Open();
    }
}
