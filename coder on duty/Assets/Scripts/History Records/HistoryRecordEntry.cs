using TMPro;
using UnityEngine;

public class HistoryRecordEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI starsText;


    public void SetValue(AttemptData attemptData)
    {
        dateText.text = attemptData.dateTime.ToString("MM/dd/yyyy hh:mm tt");
        nameText.text = attemptData.programmerName;
        scoreText.text = attemptData.score.ToString();
        accuracyText.text = $"{(int)attemptData.accuracy}%";
        starsText.text = ((int)attemptData.averageStars).ToString();
    }
}
