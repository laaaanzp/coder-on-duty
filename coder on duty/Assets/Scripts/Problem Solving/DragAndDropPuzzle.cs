using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropPuzzle : MonoBehaviour, IPuzzle
{
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private Button closeButton;

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
    [SerializeField] private float targetTime = 180.0f;

    private Ticket ticket;

    private int currentTimeInInt = 180;
    private bool hasSubmitted;

    public TaskScoreModel taskScoreModel { get; set; }
    public Action onSubmitOrFinish { get; set; }
    public int totalCorrect { get; set; }
    public int timeRemaining { get; set; }
    public int totalSlots { get; set; }

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
        totalSlots = 0;
        taskScoreModel = new TaskScoreModel();

        string[] lines = problem.Split('\n');
        List<string> choices = new List<string>(answers);

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
                    totalSlots++;
                    string answer = token.Replace("NODE_SLOT:", "");
                    choices.Add(answer);
                }
            }
        }

        foreach (string choice in Tools.ShuffleArray(choices.ToArray()))
        {
            if (choice == "")
                continue;

            AddAnswer(choice);
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

    public void Close()
    {
        modalControl.Close();
    }

    public void CheckAnswers()
    {
        hasSubmitted = true;
        StartCoroutine(_CheckAnswers());
    }

    private IEnumerator _CheckAnswers()
    {
        SlotNode[] slotNodes = GetComponentsInChildren<SlotNode>(includeInactive: true);

        totalCorrect = 0;
        timeRemaining = Mathf.FloorToInt(targetTime);

        foreach (SlotNode slotNode in slotNodes)
        {
            if (slotNode.IsAnswerCorrect())
            {
                slotNode.HighlightCorrect();
                totalCorrect++;
                AudioController.PlayNodeSlotCorrect();
            }
            else
            {
                slotNode.HighlightIncorrect();
                AudioController.PlayNodeSlotIncorrect();
            }

            yield return new WaitForSeconds(0.5f);
        }

        taskScoreModel.totalCorrectAnswers = totalCorrect;
        taskScoreModel.totalAnswers = totalSlots;

        if (totalCorrect < (float)totalSlots / 2)
        {
            ticket.isFixed = false;
            taskScoreModel.isFixed = false;
        }
        else
        {
            int score = Mathf.FloorToInt(targetTime) * totalCorrect;
            taskScoreModel.score = score;
            ScoreManager.AddScore(score);
            ticket.isFixed = true;
            taskScoreModel.isFixed = true;
        }

        ScoreManager.finishedDevices++;
        closeButton.interactable = true;
    }

    public void CloseWindow()
    {
        if (taskScoreModel.isFixed)
            AudioController.PlayFixedDevice();

        else
            AudioController.PlayUnfixedDevice();

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
        if (hasSubmitted)
        {
            return;
        }
        targetTime -= Time.deltaTime;

        int newTimeInInt = (int)targetTime;

        if (newTimeInInt != currentTimeInInt)
        {
            currentTimeInInt = newTimeInInt;
            OnTimeSecond();
        }

        targetTime = MathF.Max(targetTime, 0);
        UpdateTimeDisplay();

        if (targetTime == 0)
        {
            CheckAnswers();
        }
    }

    private void OnTimeSecond()
    {
        if (currentTimeInInt <= 10)
        {
            AudioController.PlayBeep();
        }
    }
}
