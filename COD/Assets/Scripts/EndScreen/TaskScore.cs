using TMPro;
using UnityEngine;

public class TaskScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI correctAnswersText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private CanvasGroup canvasGroup;


    public void Show(TaskScoreModel taskScore)
    {
        taskNameText.text = taskScore.name;
        taskNameText.color = !taskScore.isFixed ? Color.red : Color.green;
        scoreText.text = $"<b>Score:</b> {taskScore.score}";
        correctAnswersText.text = $"<b>Answers:</b> {taskScore.totalCorrectAnswers}/{taskScore.totalAnswers}";
        accuracyText.text = $"<b>Accuracy:</b> {Mathf.RoundToInt(taskScore.accuracy)}%";

        canvasGroup.LeanAlpha(1f, 0.5f);
    }
}
