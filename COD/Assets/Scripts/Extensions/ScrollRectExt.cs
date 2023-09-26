using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExt
{
    public static void ScrollToTop(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public static void ScrollToTop(this ScrollRect scrollRect, float duration)
    {
        Vector2 normalizedPosition = scrollRect.normalizedPosition;

        LeanTween.value(normalizedPosition.y, 1, duration).setOnUpdate((float v) =>
        {
            Debug.Log(v);
            scrollRect.normalizedPosition = new Vector2(0, v);
        });
    }

    public static void ScrollToBottom(this ScrollRect scrollRect, float duration)
    {
        Vector2 normalizedPosition = scrollRect.normalizedPosition;

        LeanTween.value(normalizedPosition.y, 0, duration).setOnUpdate((float v) =>
        {
            scrollRect.normalizedPosition = new Vector2(0, v);
        });
    }
}
