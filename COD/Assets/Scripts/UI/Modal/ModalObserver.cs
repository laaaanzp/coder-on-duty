using UnityEngine;

public class ModalObserver : MonoBehaviour
{
    public static int activeModalCount;


    void Awake()
    {
        foreach (ModalControl modalControl in GetComponentsInChildren<ModalControl>(includeInactive: true))
        {
            // modalControl.Initialize();
        }
    }

    void Update()
    {
        Cursor.lockState = activeModalCount == 0 ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
