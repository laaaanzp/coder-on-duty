using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsModal : MonoBehaviour
{
    [SerializeField] private ModalControl statisticsModal;

    [SerializeField] private GameObject titleObject;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI ratingsText;

    [Header("Stars")]
    [SerializeField] private CanvasGroup artCanvasGroup;
    [SerializeField] private CanvasGroup starsCanvasGroup;
    [SerializeField] private Image[] stars;

    [Header("Buttons")]
    [SerializeField] private CanvasGroup[] buttonsCanvasGroup;

    private int totalTime, totalScore;
    private float overallAccuracy, overallStars;


    public void Open(int time, int score, float accuracy, float stars)
    {
        totalTime = time;
        totalScore = score;
        overallAccuracy = accuracy;
        overallStars = stars;

        statisticsModal.Open();

        AnimateTitle();
    }

    private void AnimateTitle()
    {
        titleObject.LeanScale(new Vector3(1, 1, 1), 0.3f)
            .setDelay(0.3f)
            .setOnComplete(AnimateArt);
    }

    private void AnimateArt()
    {
        artCanvasGroup
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.75f)
            .setOnComplete(AnimateScore);
    }

    private void AnimateScore()
    {
        scoreText.GetComponent<CanvasGroup>()
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(() =>
            {
                LeanTween.value(0, totalScore, 1f).setDelay(0.3f).setOnUpdate(score =>
                {
                    scoreText.text = $"<b>Total Score:</b> {(int)score}";
                }).setOnComplete(AnimateTime);
            });
    }

    private void AnimateTime()
    {
        timeText.GetComponent<CanvasGroup>()
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(() =>
            {
                LeanTween.value(0, totalTime, 1f).setDelay(0.3f).setOnUpdate(fSeconds =>
                {
                    int seconds = (int)fSeconds;
                    int minutes = seconds / 60;
                    seconds = seconds % 60;

                    string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

                    timeText.text = $"<b>Total Time:</b> {timeString}";
                })
                .setOnComplete(AnimateAccuracy);
            });
    }

    private void AnimateAccuracy()
    {
        accuracyText.GetComponent<CanvasGroup>()
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(() =>
            {
                LeanTween.value(0, overallAccuracy, 1f).setDelay(0.3f).setOnUpdate((float accuracy) =>
                {
                    accuracyText.text = $"<b>Overall Accuracy:</b> {accuracy:F2}%";
                }).setOnComplete(AnimateStars);
            });
    }

    private void AnimateStars()
    {
        for (int i = 0; i < overallStars; i++)
        {
            stars[i].color = Color.white;
        }

        switch (overallStars)
        {
            case 2:
                ratingsText.text = "<b>Skill Level:</b> Advanced";
                break;
            case 3:
                ratingsText.text = "<b>Skill Level:</b> Expert";
                break;
        }

        starsCanvasGroup
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(AnimateRatings);
    }

    private void AnimateRatings()
    {
        ratingsText.GetComponent<CanvasGroup>()
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

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }
}
