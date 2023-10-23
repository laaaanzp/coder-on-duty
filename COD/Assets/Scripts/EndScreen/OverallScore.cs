using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverallScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI correctAnswersText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] starImages;


    public void Show(int score, int correctAnswers, int totalAnswers, float accuracy, int stars)
    {
        scoreText.text = $"<b>Score:</b> {score}";
        correctAnswersText.text = $"<b>Answers:</b> {correctAnswers}/{totalAnswers}";
        accuracyText.text = $"<b>Accuracy:</b> {Mathf.RoundToInt(accuracy)}%";

        for (int i = 0; i < stars; i++)
        {
            starImages[i].color = new Color(1f, 1f, 1f, 1f);
        }

        canvasGroup.LeanAlpha(1f, 0.5f);
    }
}
