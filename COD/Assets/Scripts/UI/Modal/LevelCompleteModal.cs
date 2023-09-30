using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteModal : MonoBehaviour
{
    [SerializeField] private ModalControl levelCompleteModal;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;


    public void Open()
    {
        timeText.text = $"<b>Time:</b> {LevelTimer.GetTimeAsString()}";
        scoreText.text = $"<b>Score:</b> {ScoreTracker.score}";

        levelCompleteModal.Open();
    }

    public void NextLevel()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        LanguageDatabase.Play(languageName);
    }

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }
}
