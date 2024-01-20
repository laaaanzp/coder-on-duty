using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FindAndSearchHoverFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.25f;
    [SerializeField] private bool isTop;

    private bool isHovered;


    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    void Update()
    {
        if (!isHovered) return;

        Vector2 normalizedPos = scrollRect.normalizedPosition;

        float scrollDelta = (isTop ? 0.25f : -0.25f) * Time.deltaTime;

        normalizedPos.y += scrollDelta;
        normalizedPos.y = Mathf.Clamp01(normalizedPos.y);

        scrollRect.normalizedPosition = normalizedPos;
    }
}
