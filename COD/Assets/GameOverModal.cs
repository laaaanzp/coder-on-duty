using TMPro;
using UnityEngine;

public class GameOverModal : MonoBehaviour
{
    [SerializeField] private ModalControl gameOverModal;

    [SerializeField] private GameObject titleObject;

    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Buttons")]
    [SerializeField] private CanvasGroup[] buttonsCanvasGroup;

    public void Open()
    {
        gameOverModal.Open();

        AnimateTitle();
    }

    private void AnimateTitle()
    {
        titleObject.LeanScale(new Vector3(1, 1, 1), 0.7f)
            .setDelay(0.3f)
            .setOnComplete(AnimateFeedback);
    }

    private void AnimateFeedback()
    {
        feedbackText.GetComponent<CanvasGroup>()
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(AnimateButtons);
    }

    private void AnimateButtons()
    {
        foreach (CanvasGroup buttonCanvasGroup in buttonsCanvasGroup)
        {
            buttonCanvasGroup.LeanAlpha(1f, 0.5f).setDelay(0.5f);
        }
    }

    public void Restart()
    {
        SceneSwitcher.Restart();
    }

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }
}
