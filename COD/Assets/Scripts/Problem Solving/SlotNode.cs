using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotNode : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string correctAnswer;
    private Vector2 highlightEffectDistance = new Vector2(3, 3);

    private float minWidth, minHeight;
    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private UnityEngine.UI.Outline outline;
    private Color effectColor;
    private Vector2 effectDistance;

    private bool isChecked;

    
    void Awake()
    {
        effectColor = outline.effectColor;
        effectDistance = outline.effectDistance;

        minWidth = layoutElement.minWidth;
        minHeight = layoutElement.minHeight;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        AnswerNode draggableItem = dropped.GetComponent<AnswerNode>();

        if (draggableItem == null)
            return;

        if (transform.childCount != 0)
            transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);

        draggableItem.parentAfterDrag = transform;

        // FixSize();

        Unhighlight();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AnswerNode.current == null)
            return;

        Rect rect = AnswerNode.current.GetComponent<RectTransform>().rect;

        layoutElement.minWidth = rect.width;
        layoutElement.minHeight = rect.height;

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // FixSize();

        Unhighlight();
    }

    private void Highlight()
    {
        if (isChecked)
            return;

        outline.effectColor = Color.yellow;
        outline.effectDistance = highlightEffectDistance;
    }

    private void Unhighlight()
    {
        if (isChecked)
            return;

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
        isChecked = true;

        if (transform.childCount == 0) 
            return false;

        AnswerNode answerNode = transform.GetChild(0).GetComponent<AnswerNode>();
        answerNode.SetUninteractable();

        if (answerNode.answer != correctAnswer)
            answerNode.correctAnswer = correctAnswer;

        return answerNode.answer == correctAnswer;
    }

    public void ResetNodeParent()
    {
        if (transform.childCount == 0) return;

        AnswerNode answerNode = transform.GetChild(0).GetComponent<AnswerNode>();
        answerNode?.ResetNodeParent();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void FixSize()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(_FixSize());
    }    

    IEnumerator _FixSize()
    {
        yield return null;

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
    }

    void OnTransformChildrenChanged()
    {
        if (!gameObject.activeInHierarchy)
            return;

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
    }
}
