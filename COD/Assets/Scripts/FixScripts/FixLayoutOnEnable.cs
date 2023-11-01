using UnityEngine;
using UnityEngine.UI;

public class FixLayoutOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
