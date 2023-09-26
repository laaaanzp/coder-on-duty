using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    public float rotationY = 145;
    public float duration = 1.5f;

    private Interactable interactable;


    void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactionName = "Open";
    }

    public void Interact()
    {
        if (interactable.interactionName == "Open")
            Open();

        else
            Close();
    }

    private void Open()
    {
        interactable.isInteractable = false;
        float rotation = gameObject.transform.eulerAngles.y - rotationY;

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

        gameObject.LeanRotateY(rotation, duration).
            setOnComplete(() =>
            {
                interactable.interactionName = "Open";
                interactable.isInteractable = true;
            });
    }
}
