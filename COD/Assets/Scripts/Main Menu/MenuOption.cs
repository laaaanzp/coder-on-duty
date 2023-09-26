using UnityEngine;

public class MenuOption : MonoBehaviour
{
    private CanvasGroup canvasGroup;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.LeanAlpha(0f, 0f);
    }

    public void Show()
    {
        canvasGroup.LeanAlpha(1f, 0.75f);
    }

    public void OnClick()
    {
        Debug.Log($"{name}: OnClick");
    }
}
