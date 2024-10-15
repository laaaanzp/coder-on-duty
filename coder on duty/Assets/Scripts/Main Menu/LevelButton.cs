using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    [TextArea(5, 10)] public string levelDescription;

    private Color defaultNormalColor;
    private Color selectedNormalColor;


    void Awake()
    {
        button = GetComponent<Button>();

        defaultNormalColor = button.colors.normalColor;
        selectedNormalColor = button.colors.selectedColor;
    }

    public void SetSelected()
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = selectedNormalColor;
        button.colors = colorBlock;
    }

    public void SetUnselected()
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = defaultNormalColor;
        button.colors = colorBlock;
    }
}
