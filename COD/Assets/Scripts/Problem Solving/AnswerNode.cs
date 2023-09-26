using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public string answer;

    private CanvasGroup canvasGroup;
    private Transform defaultParent;

    public static AnswerNode current;


    void Awake()
    {
        defaultParent = transform.parent;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ResetNodeParent()
    {
        transform.SetParent(defaultParent);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        current = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        transform.SetParent(parentAfterDrag);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentAfterDrag.GetComponent<RectTransform>());

        canvasGroup.blocksRaycasts = true;
        current = null;
    }

    void OnValidate()
    {
        if (answer == "")
            answer = "UNASSIGNED VALUE";

        name = $"AnsNode: {answer}";
        GetComponentInChildren<TextMeshProUGUI>().text = answer;
    }
}
