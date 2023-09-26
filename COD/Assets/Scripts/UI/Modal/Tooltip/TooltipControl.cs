using UnityEngine;

public class TooltipControl : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;

    private static Tooltip instance;


    void Awake()
    {
        if (instance != null)
            return;

        instance = tooltip;
    }

    public static void Open(string title, string description)
    {
        instance.Open(title, description);
    }

    public static void Close()
    {
        instance.Close();
    }
}
