using TMPro;
using UnityEngine;

public class InteractOverlayControl : MonoBehaviour
{
    public TextMeshProUGUI interactableNameText;

    public CanvasGroup interactionBarCanvasGroup;
    public TextMeshProUGUI keybindText;
    public TextMeshProUGUI interactionText;


    public void Show(Interactable interactable)
    {
        float interactionBarAlpha = interactable.isInteractable ? 1.0f : 0.5f;

        interactionBarCanvasGroup.alpha = interactionBarAlpha;

        interactableNameText.text = interactable.gameObject.name;
        keybindText.text = interactable.keybind.ToString();
        interactionText.text = interactable.interactionName;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
