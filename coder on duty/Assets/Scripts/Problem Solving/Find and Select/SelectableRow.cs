using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableRow : MonoBehaviour, IPointerClickHandler
{
    private Vector2 highlightEffectDistance = new Vector2(3, 3);

    [SerializeField] private UnityEngine.UI.Outline outline;
    [SerializeField] private TextMeshProUGUI rowText;

    public bool isAnswer;
    private bool isSelected;
    private bool isChecked;

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

    public void HighlightSelected()
    {
        outline.effectColor = new Color(1, 0.5f, 0f, 1f); // orange
        outline.effectDistance = highlightEffectDistance;
    }

    public void HighlightUnselected()
    {
        outline.effectDistance = new Vector2(0, 0);
    }

    public bool IsAnswerCorrect()
    {
        isChecked = true;
        return isSelected == isAnswer;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isChecked)
            return;

        if (isSelected)
        {
            HighlightUnselected();
            isSelected = false;
            FindAndSelect.totalSelected--;
        }
        else
        {
            if (FindAndSelect.maxTotalSelected == FindAndSelect.totalSelected)
                return;

            HighlightSelected();
            isSelected = true;
            FindAndSelect.totalSelected++;
        }
    }

    public void SetText(string text)
    {
        rowText.text = text;
    }
}
