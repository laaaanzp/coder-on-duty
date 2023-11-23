using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseModalControl : MonoBehaviour
{
    [SerializeField] private PauseModal pauseModal;


    void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0 || sceneIndex == 3)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !TicketManager.isLevelCompleted)
        {
            pauseModal.Toggle();
        }
    }
}
