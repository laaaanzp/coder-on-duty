using UnityEngine;
using UnityEngine.UI;

public class KeepScroll : MonoBehaviour 
{
    private bool justDragged = false;

    private ScrollRect scrollRect;
    private Vector2 normalizedPosition;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        normalizedPosition = scrollRect.normalizedPosition = new Vector2(1, 1);
    }

    void Update()
    {
        if (AnswerNode.current != null)
        {
            scrollRect.enabled = false;
            justDragged = true;
            scrollRect.normalizedPosition = normalizedPosition;
        }
        else
        {
            scrollRect.enabled = true;
            if (justDragged)
            {
                scrollRect.normalizedPosition = normalizedPosition;
                justDragged = false;
            }
            else
            {
                normalizedPosition = scrollRect.normalizedPosition;
            }
        }
    }
}
