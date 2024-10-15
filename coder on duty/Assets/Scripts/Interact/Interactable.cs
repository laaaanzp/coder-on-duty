using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public KeyCode keybind = KeyCode.E;
    public string interactionName = "Interact";
    public bool isInteractable = true;

    public UnityEvent interactionEvent;
    

    public void Interact()
    {
        interactionEvent?.Invoke();
    }
}
