using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteModal : MonoBehaviour
{
    [SerializeField] private ModalControl levelCompleteModal;
    [SerializeField] private StatisticsModal statisticsModalControl;

    [SerializeField] private GameObject titleObject;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("Stars")]
    [SerializeField] private CanvasGroup starsCanvasGroup;
    [SerializeField] private Image[] stars;

    [Header("Buttons")]
    [SerializeField] private CanvasGroup[] buttonsCanvasGroup;

    private int totalTime, totalScore;
    private float totalAccuracy, totalStars;
    private bool isLastLevel;

    public void Open(int time, int score, float accuracy, float stars, bool isLastLevel)
    {
        this.totalTime = time;
        this.totalScore = score;
        this.totalAccuracy = accuracy;
        this.totalStars = stars;
        this.isLastLevel = isLastLevel;
        levelCompleteModal.Open();

        AnimateTitle();
    }

    private void AnimateTitle()
    {
        titleObject.LeanScale(new Vector3(1, 1, 1), 0.3f)
            .setDelay(0.3f)
            .setOnComplete(AnimateScore);
    }

    private void AnimateScore()
    {
        scoreText.GetComponent<CanvasGroup>()
            .LeanAlpha(1f, 0.5f)
            .setDelay(0.3f)
            .setOnComplete(() =>
            {
                LeanTween.value(0, ScoreManager.score, 1f).setDelay(0.3f).setOnUpdate(score =>
                {
                    scoreText.text = $"<b>Score:</b> {(int)score}";
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
                LeanTween.value(0, LevelTimer.GetTimeInSeconds(), 1f).setDelay(0.3f).setOnUpdate(fSeconds =>
                {
                    int seconds = (int)fSeconds;
                    int minutes = seconds / 60;
                    seconds = seconds % 60;

                    string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

                    timeText.text = $"<b>Time:</b> {timeString}";
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
                LeanTween.value(0, ScoreManager.accuracy, 1f).setDelay(0.3f).setOnUpdate((float accuracy) =>
                {
                    accuracyText.text = $"<b>Accuracy:</b> {accuracy:F2}%";
                }).setOnComplete(AnimateStars);
            });
    }

    private void AnimateStars()
    {
        int totalStars = 1;

        if (ScoreManager.accuracy == 100f)
        {
            totalStars++;
        }

        if (LevelTimer.onTime)
        {
            totalStars++;
        }

        for (int i = 0; i < totalStars; i++)
        {
            stars[i].color = Color.white;
        }

        starsCanvasGroup
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

    public void Proceed()
    {
        if (isLastLevel)
        {
            ShowFinalData();
        }
        else
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        LanguageDatabase.Play(languageName);
    }

    private void ShowFinalData()
    {
        levelCompleteModal.Close();
        statisticsModalControl.Open(totalTime, totalScore, totalAccuracy, totalStars);
    }

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }
}
