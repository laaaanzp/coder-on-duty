using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrosswordPiece : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    private char value;

    public void SetValue(char value)
    {
        if (value != '-')
            this.value = value;

        else
            image.color = new Color(0.137f, 0.137f, 0.137f, 0.5f);
    }

    public void RevealText()
    {
        text.text = value.ToString();
    }
}
