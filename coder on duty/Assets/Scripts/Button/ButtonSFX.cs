using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioController.PlayButtonClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioController.PlayButtonHover();
    }
}
