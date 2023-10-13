using System;
using System.Net.Sockets;
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
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;

    [Header("Score")]
    [SerializeField] private float targetTime = 120.0f;

    public Action onSubmitOrFinish;
    private Ticket ticket;
    public int totalCorrect = 0;
    public int timeRemaining = 0;
    private bool isOnFinishCalled;
    
    public void SetTicket(Ticket ticket)
    {
        this.ticket = ticket;
    }

    void Awake()
    {
        UpdateTimeDisplay();
    }

    public void Initialize(string problem, string[] answers, string output)
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

                if (token.StartsWith("NODE_SLOT:"))
                {
                    string answer = token.Replace("NODE_SLOT:", "");
                    AddAnswer(answer);
                }
            }
        }

        foreach (string answer in answers)
        {
            if (answer == "")
            {
                continue;
            }

            AddAnswer(answer);
        }

        outputText.text = output;
    }

    private void AddAnswer(string answer)
    {
        GameObject answerObject = Instantiate(answerNodePrefab, answersContainer);
        AnswerNode answerNode = answerObject.GetComponent<AnswerNode>();

        answerNode.SetAnswer(answer.Trim());
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

        totalCorrect = 0;
        timeRemaining = Mathf.FloorToInt(targetTime);

        ticket.isFinished = true;

        foreach (SlotNode slotNode in slotNodes)
        {
            if (slotNode.IsAnswerCorrect())
                totalCorrect++;
        }

        ScoreManager.totalCorrect += totalCorrect;
        ScoreManager.totalSlots += slotNodes.Length;

        if (totalCorrect < 3)
        {
            ticket.isFixed = false;
        }
        else
        {
            int score = Mathf.FloorToInt(targetTime) * 10;
            ScoreManager.AddScore(score);
            ScoreManager.fixedDevices++;
            ticket.isFixed = true;
        }
        ScoreManager.finishedDevices++;
        onSubmitOrFinish?.Invoke();
        modalControl.Close();
    }

    public void ClearAnswers()
    {
        SlotNode[] slotNodes = GetComponentsInChildren<SlotNode>();

        foreach (SlotNode slotNode in slotNodes)
        {
            slotNode.ResetNodeParent();
        }
    }

    private void Update()
    {
        targetTime -= Time.deltaTime;

        targetTime = MathF.Max(targetTime, 0);
        UpdateTimeDisplay();

        if (targetTime == 0)
        {
            if (!isOnFinishCalled)
            {
                onSubmitOrFinish?.Invoke();
                modalControl.Close();
            }
            isOnFinishCalled = true;
        }
    }
}
