using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] private string languageName;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Image[] stars;
    [SerializeField] private Button playButton;


    void OnEnable()
    {
        playButton.gameObject.SetActive(false);
        foreach (Image star in stars)
        {
            star.gameObject.SetActive(false);
        }

        LevelData levelData = LanguageDatabase.GetInstance(languageName).GetLevelDataByName(levelNameText.text);
        SceneReference scene = LanguageDatabase.GetInstance(languageName).scenes[LanguageDatabase.GetInstance(languageName).currentLevel - 1];

        if (scene.SceneName == levelNameText.text)
        {
            SetAsCurrent();
        }
        else if (levelData.stars == 0)
        {
            SetAsLocked();
        }
        else
        {
            SetDisplay(levelData);
        }
    }

    public void Play()
    {
        LanguageDatabase.Play(languageName);
    }

    private void SetAsLocked()
    {
        scoreText.text = "<b>Locked</b>";
        timeText.text = "";
        accuracyText.text = "";
        backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    private void SetAsCurrent()
    {
        scoreText.text = "<b>Current Level</b>";
        timeText.text = "";
        accuracyText.text = "";
        backgroundImage.color = new Color(0.79f, 0.34f, 0.34f, 1f);

        foreach (Image star in stars)
        {
            star.gameObject.SetActive(false);
        }

        playButton.gameObject.SetActive(true);
    }

    private void SetDisplay(LevelData levelData)
    {
        scoreText.text = $"<b>Score:</b> {levelData.score}";
        timeText.text = $"<b>Answers:</b> {levelData.totalCorrectAnswers}/{levelData.totalAnswers}";
        accuracyText.text = $"<b>Accuracy:</b> {levelData.accuracy:F0}%";
        backgroundImage.color = new Color(0.72f, 0.72f, 0.3f, 1f);

        int totalStars = 1;

        if (levelData.stars > 3)
        {
            totalStars = 3;
        }

        for (int i = 0; i < totalStars; i++)
        {
            stars[i].gameObject.SetActive(true);
        }
    }
}
