using UnityEngine;

public class PauseModalControl : MonoBehaviour
{
    [SerializeField] private PauseModal pauseModal;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !TicketManager.isLevelCompleted)
        {
            pauseModal.Toggle();
        }
    }
}
