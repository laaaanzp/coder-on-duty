using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseModal : MonoBehaviour
{
    [SerializeField] private ModalControl pauseModalControl;
    [SerializeField] private ModalControl settingsModalControl;


    public void Toggle()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // if the current scene is main menu
        {
            return;
        }

        if (gameObject.activeInHierarchy)
        {
            pauseModalControl.Close();
            settingsModalControl.Close();
        }

        else
            pauseModalControl.Open();
    }
    
    public void Resume()
    {
        pauseModalControl.Close();
    }

    public void Settings()
    {
        settingsModalControl.Open();
    }
}
