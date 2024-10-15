using UnityEngine;

public class SettingsModalControl : MonoBehaviour
{
    [SerializeField] private ModalControl settingsModal;

    private static ModalControl instance;


    void Awake()
    {
        instance = settingsModal;
    }

    public static void Open()
    {
        instance.Open();
    }
}
