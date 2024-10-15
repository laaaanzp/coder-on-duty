using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private CanvasGroup scoreCanvasGroup;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI answersText;

    [Header("Datas")]
    [SerializeField] private CanvasGroup[] canvasGroups;

    [Header("Stars")]
    [SerializeField] private Image[] stars;

    [Header("Button")]
    [SerializeField] private CanvasGroup buttonCanvasGroup;


    void Start()
    {
        StartCoroutine(AnimateEndScene());
    }

    IEnumerator AnimateEndScene()
    {
        AttemptData attemptData = DatabaseManager.instance.currentLanguage.GetLatestData();

        scoreText.text = $"<b>Score:</b> {attemptData.score}";
        accuracyText.text = $"<b>Accuracy:</b> {(int)attemptData.accuracy}%";
        answersText.text = $"<b>Answers:</b> {attemptData.totalCorrectAnswers} / {attemptData.totalAnswers}";


        for (int i = 0; i < (int)attemptData.averageStars; i++)
        {
            stars[i].color = new Color(1f, 1f, 1f, 1f);
        }

        yield return new WaitForSeconds(1f);

        titleCanvasGroup.gameObject.LeanScale(new Vector3(0.9f, 0.9f, 0.9f), 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);

        titleCanvasGroup.LeanAlpha(1f, 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);

        yield return new WaitForSeconds(1.2f);

        scoreCanvasGroup.LeanAlpha(1f, 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);

        foreach (CanvasGroup dataCanvasGroup in canvasGroups)
        {
            yield return new WaitForSeconds(0.85f);
            dataCanvasGroup.LeanAlpha(1f, 0.5f)
                .setIgnoreTimeScale(true)
                .setEase(LeanTweenType.easeInExpo);
        }

        yield return new WaitForSeconds(1.25f);

        buttonCanvasGroup.LeanAlpha(1f, 0.5f)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInExpo);
    }

    public void MainMenu()
    {
        SceneSwitcher.LoadMenu();
    }
}
