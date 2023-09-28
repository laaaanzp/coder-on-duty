using TMPro;
using UnityEngine;

public class StatisticsModal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userScoreText;
    [SerializeField] private TextMeshProUGUI userTimeText;

    [SerializeField] private TextMeshProUGUI averageScoreText;
    [SerializeField] private TextMeshProUGUI averageTimeText;

    [SerializeField] private TextMeshProUGUI scoreFindingsText;
    [SerializeField] private TextMeshProUGUI timeFindingsText;


    void Start()
    {
        DatabaseManager.instance.currentLanguage.FetchUserLatestAttempt(userData =>
        {
            userScoreText.text = $"<b>Score</b>: {userData.score}";

            int minutes = userData.time / 60;
            int seconds = userData.time % 60;

            userTimeText.text = string.Format("<b>Time</b>: {0:00}m {1:00}s", minutes, seconds);


            DatabaseManager.instance.currentLanguage.FetchLatestAverageAttempt(averageData =>
            {
                averageScoreText.text = $"<b>Score</b>: {averageData.score}";

                int minutes = averageData.time / 60;
                int seconds = averageData.time % 60;

                averageTimeText.text = string.Format("<b>Time</b>: {0:00}m {1:00}s", minutes, seconds);

                int scoreDifference = userData.score - averageData.score;
                float scorePercentageDifference = (float)scoreDifference / (float)averageData.score * 100;

                if (scorePercentageDifference > 0)
                {
                    scoreFindingsText.text = $"Your score is {scorePercentageDifference:F2}% higher than the average player.";
                }
                else if (scorePercentageDifference < 0)
                {
                    scoreFindingsText.text = $"Your score is {Mathf.Abs(scorePercentageDifference):F2}% lower than the average player.";
                }
                else
                {
                    scoreFindingsText.text = "Your score matches the average player.";
                }

                int timeDifference = userData.time - averageData.time;
                float timePercentageDifference = ((float)(userData.time - averageData.time) / averageData.time) * 100;;

                if (timePercentageDifference < 0)
                {
                    timeFindingsText.text = $"You completed the game {Mathf.Abs(timePercentageDifference):F2}% faster than the average player.";
                }
                else if (timePercentageDifference > 0)
                {
                    timeFindingsText.text = $"You took {timePercentageDifference:F2}% more time than the average player.";
                }
                else
                {
                    timeFindingsText.text = "Your completion time matches the average player.";
                }
            }, null);

        }, null);
    }
}
