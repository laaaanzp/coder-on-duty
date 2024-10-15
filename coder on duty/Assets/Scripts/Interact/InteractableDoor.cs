using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    public float rotationY = 145;
    public float duration = 1.5f;

    [SerializeField] private int levelRequired = 1;
    [SerializeField] private string errorMessage;

    private Interactable interactable;


    void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactionName = "Open";
    }

    public void Interact()
    {
        if (DatabaseManager.instance.currentLanguage.currentLevel < levelRequired)
        {
            NPCBoss.Say(errorMessage);
            return;
        }

        if (interactable.interactionName == "Open")
            Open();

        else
            Close();
    }

    private void Open()
    {
        interactable.isInteractable = false;
        float rotation = gameObject.transform.eulerAngles.y - rotationY;
        AudioController.PlayDoorInteract();
        gameObject.LeanRotateY(rotation, duration).
            setOnComplete(() =>
            {
                interactable.interactionName = "Close";
                interactable.isInteractable = true;
            });
    }

    private void Close()
    {
        interactable.isInteractable = false;
        float rotation = gameObject.transform.eulerAngles.y + rotationY;
        AudioController.PlayDoorInteract();
        gameObject.LeanRotateY(rotation, duration).
            setOnComplete(() =>
            {
                interactable.interactionName = "Open";
                interactable.isInteractable = true;
            });
    }
}
