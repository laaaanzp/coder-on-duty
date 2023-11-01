using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotNode : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string correctAnswer;
    private Vector2 highlightEffectDistance = new Vector2(3, 3);

    private float minWidth, minHeight;
    private LayoutElement layoutElement;

    [SerializeField] private UnityEngine.UI.Outline outline;
    private Color effectColor;
    private Vector2 effectDistance;

    
    void Awake()
    {
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
        if (transform.childCount != 0)
        {
            Rect childRect = transform.GetChild(0).GetComponent<RectTransform>().rect;

            layoutElement.minWidth = childRect.width;
            layoutElement.minHeight = childRect.height;
        }
        else
        {
            layoutElement.minWidth = minWidth;
            layoutElement.minHeight = minHeight;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Unhighlight();
    }

    private void Highlight()
    {
        outline.effectColor = Color.yellow;
        outline.effectDistance = highlightEffectDistance;
    }

    private void Unhighlight()
    {
        outline.effectColor = effectColor;
        outline.effectDistance = effectDistance;
    }

    public void HighlightCorrect()
    {
        outline.effectColor = Color.green;
        outline.effectDistance = highlightEffectDistance;
    }

    public void HighlightIncorrect()
    {
        outline.effectColor = Color.red;
        outline.effectDistance = highlightEffectDistance;
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
