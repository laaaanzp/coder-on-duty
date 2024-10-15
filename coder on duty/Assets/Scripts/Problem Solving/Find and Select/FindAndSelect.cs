using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindAndSelect : MonoBehaviour, IPuzzle
{
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private Button closeButton;
    [SerializeField] private bool isTraining;

    [Header("Problem")]
    [SerializeField] private GameObject selectableRowPrefab;
    [SerializeField] private Transform problemSolvingRowContainer;

    [Header("Texts")]
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

    public static int totalSelected;
    public static int maxTotalSelected;

    public void SetTicket(Ticket ticket)
    {
        this.ticket = ticket;
    }

    void Awake()
    {
        UpdateTimeDisplay();
    }

    public void Initialize(string[] lines)
    {
        totalSlots = 0;
        totalSelected = maxTotalSelected = 0;
        taskScoreModel = new TaskScoreModel();

        foreach (string line in lines)
        {
            GameObject slotRowObject = Instantiate(selectableRowPrefab, problemSolvingRowContainer);
            SelectableRow selectableRow = slotRowObject.GetComponent<SelectableRow>();

            if (line.StartsWith(">>"))
            {
                selectableRow.SetText(line.Remove(0, 2));
                selectableRow.isAnswer = true;
                totalSlots++;
                maxTotalSelected++;
            }
            else
            {
                selectableRow.SetText(line);
                selectableRow.isAnswer = false;
            }
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
        SelectableRow[] selectableRows = GetComponentsInChildren<SelectableRow>(includeInactive: true);

        totalCorrect = 0;
        timeRemaining = Mathf.FloorToInt(targetTime);

        foreach (SelectableRow selectableRow in selectableRows)
        {
            if (!selectableRow.isAnswer)
                continue;

            if (selectableRow.IsAnswerCorrect())
            {
                selectableRow.HighlightCorrect();
                totalCorrect++;
                AudioController.PlayNodeSlotCorrect();
            }
            else
            {
                selectableRow.HighlightIncorrect();
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
        if (hasSubmitted || isTraining)
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
