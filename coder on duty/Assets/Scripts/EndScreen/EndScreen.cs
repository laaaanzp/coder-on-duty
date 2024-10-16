using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI titleShadowText;
    [SerializeField] private CanvasGroup buttonsCanvasGroup;

    [Header("Task Scores")]
    [SerializeField] private TaskScore[] taskScores;

    [Header("Overall")]
    [SerializeField] private OverallScore overallScore;

    [Header("Buttons")]
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject proceedButton;

    private int totalScore = 0;
    private int totalCorrectAnswers = 0;
    private int totalAnswers = 0;
    private int stars = 0;

    public void ShowGameOver(TaskScoreModel[] taskScores)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string[] types = { "DragAndArrangePuzzle", "DragAndDropPuzzle", "FindAndSelect" };
        foreach (string type in types)
        {
            PlayerPrefs.SetInt($"{languageName}-{type}-total-correct", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-time-remaining", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-total-slots", -1);
        }

        PlayerPrefs.Save();

        modalControl.Open();
        titleText.text = "GAME OVER!";
        titleShadowText.text = "GAME OVER!";
        titleText.color = Color.red;

        proceedButton.SetActive(false);
        StartCoroutine(_Show(taskScores));
    }

    public void ShowLevelComplete(TaskScoreModel[] taskScores)
    {
        switch (DatabaseManager.instance.currentLanguage.currentLevel)
        {
            case 9999:
                titleText.text = "PROMOTED TO PROBITIONARY!";
                titleShadowText.text = "PROMOTED TO PROBITIONARY!";
                break;
            case 3:
                titleText.text = "PROMOTED TO REGULAR!";
                titleShadowText.text = "PROMOTED TO REGULAR!";
                break;
        }
        modalControl.Open();
        restartButton.SetActive(false);
        StartCoroutine(_Show(taskScores));
    }

    public void Restart()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string[] types = { "DragAndArrangePuzzle", "DragAndDropPuzzle", "FindAndSelect" };
        foreach (string type in types)
        {
            PlayerPrefs.SetInt($"{languageName}-{type}-total-correct", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-time-remaining", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-total-slots", -1);
        }

        PlayerPrefs.Save();

        SceneSwitcher.Restart();
    }

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }

    public void Proceed()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string[] types = { "DragAndArrangePuzzle", "DragAndDropPuzzle", "FindAndSelect" };
        foreach (string type in types)
        {
            PlayerPrefs.SetInt($"{languageName}-{type}-total-correct", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-time-remaining", -1);
            PlayerPrefs.SetInt($"{languageName}-{type}-total-slots", -1);
        }

        PlayerPrefs.Save();

        DatabaseManager.instance.currentLanguage.SetCurrentLevelData(totalScore, stars, totalCorrectAnswers, totalAnswers);

        if (DatabaseManager.instance.currentLanguage.isPlayingTheLastLevel)
        {
            SceneSwitcher.LoadEndScreen();
            DatabaseManager.instance.currentLanguage.LevelUp();
        }
        else
        {
            DatabaseManager.instance.currentLanguage.LevelUp();
            LanguageDatabase.Play(DatabaseManager.instance.currentLanguage.languageName);
        }
    }

    IEnumerator _Show(TaskScoreModel[] taskScoreModels)
    {
        yield return new WaitForSeconds(0.5f);

        titleCanvasGroup.gameObject.LeanScale(new Vector3(1f, 1f, 1f), 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);

        titleCanvasGroup.LeanAlpha(1f, 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);

        yield return new WaitForSeconds(1.2f);


        for (int i = 0; i < 3; i++)
        {
            taskScores[i].Show(taskScoreModels[i]);
            totalScore += taskScoreModels[i].score;
            totalCorrectAnswers += taskScoreModels[i].totalCorrectAnswers;
            totalAnswers += taskScoreModels[i].totalAnswers;

            if (taskScoreModels[i].isFixed)
            {
                stars++;
            }

            yield return new WaitForSeconds(0.75f);
        }

        totalScore += ScoreManager.additionalScore;

        float overallAccuracy = ((float)totalCorrectAnswers / (float)totalAnswers) * 100;

        overallScore.Show(totalScore, totalCorrectAnswers, totalAnswers, overallAccuracy, stars);

        yield return new WaitForSeconds(1.2f);

        buttonsCanvasGroup.LeanAlpha(1f, 0.5f);
    }
}
