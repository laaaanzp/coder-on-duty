using UnityEngine;

public class LevelSelectionControl : MonoBehaviour
{
    [SerializeField] private ModalControl levelSelectionModal;

    private static LevelSelectionControl instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static void Open()
    {
        instance.levelSelectionModal.Open();
    }
}
