using UnityEngine;

public class UserManualManager : MonoBehaviour
{
    [SerializeField] private ModalControl userManualModalControl;
    public static UserManualManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (ModalObserver.activeModalCount > 0)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            userManualModalControl.Open();
        }
    }

    public static void Close()
    {
        instance.userManualModalControl.Close();
    }
}
