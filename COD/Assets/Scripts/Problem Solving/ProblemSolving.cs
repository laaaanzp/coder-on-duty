using System;
using TMPro;
using UnityEngine;

public class ProblemSolving : MonoBehaviour
{
    [SerializeField] private ModalControl modalControl;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreDeductionPerMistakeText;

    [SerializeField] private float targetTime = 60.0f;

    [Header("Score")]
    [SerializeField] private int baseScore = 500;               // Score when the device is fixed
    [SerializeField] private int scoreDeductedPerMistake = 50;  // Score when the user submitted with wrong answer
    [SerializeField] private int bonusOnTimeScore = 100;        // Bonus score they get when they fixed the problem on time (targetTime).

    public Action onFix;


    void Awake()
    {
        UpdateTimeDisplay();
        UpdateScoreDisplay();
        scoreDeductionPerMistakeText.text = $"<b>Deduction Per Mistake:</b> {scoreDeductedPerMistake}";

        InvokeRepeating("DeductTime", 1f, 1f);
    }

    private void DeductTime()
    {
        targetTime -= 1f;
        UpdateTimeDisplay();

        if (targetTime == 0)
        {
            CancelInvoke("DeductTime");
        }
    }

    private void UpdateTimeDisplay()
    {
        int timeRemainingInSeconds = (int)Mathf.Round(targetTime);
        timeRemainingText.text = $"<b>Time Remaining:<b> {timeRemainingInSeconds}s";
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"<b>Score:</b> {baseScore}";
    }

    private void DeductScore()
    {
        baseScore -= scoreDeductedPerMistake;
        baseScore = baseScore > 0 ? baseScore : 0;
        UpdateScoreDisplay();
    }

    public void Open()
    {
        modalControl.Open();
    }

    public void CheckAnswers()
    {
        SlotNode[] slotNodes = GetComponentsInChildren<SlotNode>();

        int totalCorrect = 0;

        foreach (SlotNode slotNode in slotNodes)
        {
            if (slotNode.IsAnswerCorrect())
                totalCorrect++;
        }

        if (totalCorrect != slotNodes.Length)
        {
            MessageBoxControl.ShowOk(
                "ERROR", 
                $"{slotNodes.Length - totalCorrect} out of {slotNodes.Length} slots are answered incorrectly or left unanswered."
                );

            DeductScore();
            return;
        }

        modalControl.Close();

        int additionalScore = baseScore;

        if (targetTime > 0)
        {
            additionalScore += bonusOnTimeScore;
        }

        ScoreTracker.AddScore(additionalScore);

        onFix?.Invoke();
    }

    public void ClearAnswers()
    {
        SlotNode[] slotNodes = GetComponentsInChildren<SlotNode>();

        foreach (SlotNode slotNode in slotNodes)
        {
            slotNode.ResetNodeParent();
        }
    }
}
