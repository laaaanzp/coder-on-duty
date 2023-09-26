using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotNode : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string correctAnswer;
    public Color highlightEffectColor;
    public Vector2 highlightEffectDistance;

    private float minWidth, minHeight;
    private LayoutElement layoutElement;

    // Default outline value
    private UnityEngine.UI.Outline outline;
    private Color effectColor;
    private Vector2 effectDistance;

    
    void Awake()
    {
        outline = GetComponent<UnityEngine.UI.Outline>();
        effectColor = outline.effectColor;
        effectDistance = outline.effectDistance;

        layoutElement = GetComponent<LayoutElement>();

        minWidth = layoutElement.minWidth;
        minHeight = layoutElement.minHeight;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        AnswerNode draggableItem = dropped.GetComponent<AnswerNode>();

        if (transform.childCount != 0)
        {
            transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);
            LayoutRebuilder.ForceRebuildLayoutImmediate(draggableItem.parentAfterDrag.GetComponent<RectTransform>());
        }

        draggableItem.parentAfterDrag = transform;

        Unhighlight();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AnswerNode.current == null) return;

        Rect rect = AnswerNode.current.GetComponent<RectTransform>().rect;

        layoutElement.minWidth = rect.width;
        layoutElement.minHeight = rect.height;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        layoutElement.minWidth = minWidth;
        layoutElement.minHeight = minHeight;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Unhighlight();
    }

    private void Highlight()
    {
        outline.effectColor = highlightEffectColor;
        outline.effectDistance = highlightEffectDistance;
    }

    private void Unhighlight()
    {
        outline.effectColor = effectColor;
        outline.effectDistance = effectDistance;
    }

    public bool IsAnswerCorrect()
    {
        if (transform.childCount == 0) return false;

        AnswerNode answerNode = transform.GetChild(0).GetComponent<AnswerNode>();

        return answerNode.answer == correctAnswer;
    }

    public void ResetNodeParent()
    {
        if (transform.childCount == 0) return;

        AnswerNode answerNode = transform.GetChild(0).GetComponent<AnswerNode>();
        answerNode?.ResetNodeParent();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
