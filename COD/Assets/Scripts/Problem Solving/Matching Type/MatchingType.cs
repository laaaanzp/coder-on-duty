using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingType : MonoBehaviour, IPuzzle
{
    [SerializeField] private WordSearchPieceRow[] pieceRows;
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private UnityEngine.UI.Button closeButton;
    [SerializeField] private GameObject censorObject;
    [SerializeField] private GameObject leftWirePrefab;
    [SerializeField] private GameObject rightWirePrefab;
    [SerializeField] private Transform leftWireContainer;
    [SerializeField] private Transform rightWireContainer;

    public TaskScoreModel taskScoreModel { get; set; }
    public Action onSubmitOrFinish { get; set; }
    public int totalCorrect { get; set; }
    public int timeRemaining { get; set; }
    public int totalSlots { get; set; }

    private Ticket ticket;

    public static MatchingType instance;

    void Awake()
    {
        instance = this;
    }

    public void SetTicket(Ticket ticket)
    {
        this.ticket = ticket;
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
        throw new NotImplementedException();
    }

    public void Initialize(string matchingType)
    {
        closeButton.onClick.AddListener(() =>
        {
            if (ticket.isFixed)
                onSubmitOrFinish?.Invoke();
            Close();
        });

        string[] answers = matchingType.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        List<string> definitions = new List<string>();

        foreach (string answer in answers)
        {
            string[] answerSplitted = answer.Split(": ");

            string itemName = answerSplitted[0].Trim();
            string itemDefinition = answerSplitted[1].Trim();

            GameObject leftWireObj = Instantiate(leftWirePrefab, leftWireContainer);

            Wire leftWire = leftWireObj.GetComponentInChildren<Wire>();

            leftWire.SetText(itemName);
            leftWire.SetValue(itemDefinition);
            definitions.Add(itemDefinition);
            totalSlots++;
        }

        definitions = new List<string>(Tools.ShuffleArray(definitions.ToArray()));

        foreach (string definition in definitions)
        {
            GameObject rightWireObj = Instantiate(rightWirePrefab, rightWireContainer);
            Wire rightWire = rightWireObj.GetComponentInChildren<Wire>();

            rightWire.SetText(definition);
            rightWire.SetValue(definition);
            rightWire.isRight = true;
        }
    }

    public static void OnFinish()
    {
        instance._OnFinish();
    }

    public void _OnFinish()
    {
        censorObject.SetActive(true);
        ticket.isFixed = true;
        MessageBoxControl.ShowOk("MINIGAME", "You solved the minigame.\nYou can close the window now.");
    }
}
