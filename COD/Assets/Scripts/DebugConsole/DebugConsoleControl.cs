using UnityEngine;

public class DebugConsoleControl : MonoBehaviour
{
    public ModalControl consoleModalControl;
    public KeyCode consoleKey;


    void Update()
    {
        if (Input.GetKeyDown(consoleKey))
            consoleModalControl.Toggle();
    }
}
