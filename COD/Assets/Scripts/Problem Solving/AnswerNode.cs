using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public string answer;
    public bool freezeX;

    [HideInInspector] public string correctAnswer = "";

    private CanvasGroup canvasGroup;
    private Transform defaultParent;
    private bool interactable;

    public static AnswerNode current;


    void Awake()
    {
        interactable = true;
        defaultParent = transform.parent;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ResetNodeParent()
    {
        transform.SetParent(defaultParent);
    }

    public void SetUninteractable()
    {
        interactable = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable)
            return;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        current = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable)
            return;

        transform.position = new Vector3(freezeX ? transform.position.x : Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!interactable)
            return;

        transform.SetParent(parentAfterDrag);

        canvasGroup.blocksRaycasts = true;
        current = null;
    }

    public void SetAnswer(string answer)
    {
        this.answer = answer;

        name = $"AnsNode: {answer}";
        GetComponentInChildren<TextMeshProUGUI>().text = answer;
    }

    void OnValidate()
    {
        SetAnswer(answer);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (correctAnswer == "")
            return;

        TooltipControl.Open("Correct Answer", correctAnswer);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipControl.Close();
    }
}
