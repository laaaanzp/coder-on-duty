using TMPro;
using UnityEngine;

public class LevelButtonControl : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;

    private LevelButton currentlySelected;


    public void OnButtonClick(LevelButton levelButton)
    {
        descriptionText.text = levelButton.levelDescription;
        levelButton.SetSelected();

        currentlySelected?.SetUnselected();
        currentlySelected = levelButton;
    }
}
