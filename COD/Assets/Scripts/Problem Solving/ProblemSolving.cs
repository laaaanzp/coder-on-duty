using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ProblemSolving : MonoBehaviour
{
    [SerializeField] private ModalControl modalControl;

    [Header("Problem")]
    [SerializeField] private GameObject problemSolvingRowPrefab;
    [SerializeField] private Transform problemSolvingRowContainer;

    [Header("Answer")]
    [SerializeField] private GameObject answerNodePrefab;
    [SerializeField] private Transform answersContainer;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI bonusOnTimeScoreText;

    [Header("Score")]
    [SerializeField] private float targetTime = 60.0f;
    [SerializeField] private int bonusOnTimeScore = 2000;

    public Action onFix;


    void Awake()
    {
        UpdateTimeDisplay();
        bonusOnTimeScoreText.text = $"<b>Bonus On-Time Score:</b> {bonusOnTimeScore}";

        InvokeRepeating("DeductTime", 1f, 1f);

    }

    public void Initialize(string problem, string[] answers)
    {
        string[] lines = problem.Split('\n');

        foreach (string line in lines)
        {
            GameObject problemSolvingRowObject = Instantiate(problemSolvingRowPrefab, problemSolvingRowContainer);
            ProblemSolvingRowControl problemSolvingRowControl = problemSolvingRowObject.GetComponent<ProblemSolvingRowControl>(); ;

            string pattern = @"\((NODE_SLOT:[^)]+)\)";

            string[] tokens = Regex.Split(line, pattern);

            foreach (string token in tokens)
            {
                problemSolvingRowControl.AddToken(token);
            }
        }

        foreach (string answer in answers)
        {
            if (answer == "")
            {
                continue;
            }

            GameObject answerObject = Instantiate(answerNodePrefab, answersContainer);
            AnswerNode answerNode = answerObject.GetComponent<AnswerNode>();

            answerNode.SetAnswer(answer.Trim());
        }
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

            return;
        }

        modalControl.Close();

        int score = 0;

        if (targetTime > 0)
        {
            score += bonusOnTimeScore;
        }

        ScoreTracker.AddScore(score);

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
