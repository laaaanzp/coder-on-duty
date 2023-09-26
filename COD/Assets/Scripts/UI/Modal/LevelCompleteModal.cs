using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteModal : MonoBehaviour
{
    [SerializeField] private ModalControl levelCompleteModal;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject viewStatisticsButton;


    public void Open()
    {
        timeText.text = $"<b>Time:</b> {LevelTimer.GetTimeAsString()}";
        scoreText.text = $"<b>Score:</b> {ScoreTracker.score}";

        if (DatabaseManager.instance.currentLanguage.isPlayingTheLastLevel)
        {
            nextLevelButton.SetActive(false);
            viewStatisticsButton.SetActive(true);
        }

        levelCompleteModal.Open();
    }

    public void NextLevel()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        LanguageDatabase.Play(languageName);
    }

    public void ShowStatistics()
    {
        DatabaseManager.instance.currentLanguage.ReloadData();
    }
}
