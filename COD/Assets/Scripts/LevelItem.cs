using System.Collections;
using System.Collections.Generic;
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
        UserData data = LanguageDatabase.GetInstance(languageName).GetLevelDataByName(levelNameText.text);
        SceneReference scene = LanguageDatabase.GetInstance(languageName).scenes[LanguageDatabase.GetInstance(languageName).currentLevel - 1];

        if (scene.SceneName == levelNameText.text)
        {
            SetAsCurrent();
        }
        else if (data.time == 0)
        {
            SetAsLocked();
        }
        else
        {
            SetDisplay(data);
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

    private void SetDisplay(UserData data)
    {
        scoreText.text = $"<b>Score:</b> {data.score}";
        timeText.text = $"<b>Time:</b> {GetTimeAsString(data.time)}";
        accuracyText.text = $"<b>Accuracy:</b> {data.accuracy:F2}%";
        backgroundImage.color = new Color(0.72f, 0.72f, 0.3f, 1f);

        int totalStars = 1;

        if (data.stars >= 3f)
        {
            totalStars = 3;
        }
        else if (data.stars >= 2f)
        {
            totalStars = 2;
        }

        for (int i = 0; i < totalStars; i++)
        {
            stars[i].gameObject.SetActive(true);
        }
    }

    private static string GetTimeAsString(int totalTime)
    {
        int totalSeconds = totalTime;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        return timeString;
    }
}
