using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crossword : MonoBehaviour, IPuzzle
{
    [SerializeField] private CrosswordPieceRow[] pieceRows;
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private Button closeButton;

    public TaskScoreModel taskScoreModel { get; set; }
    public Action onSubmitOrFinish { get; set; }
    public int totalCorrect { get; set; }
    public int timeRemaining { get; set; }
    public int totalSlots { get; set; }

    private Ticket ticket;
    private Dictionary<string, List<Vector2Int>> answers;


    void Awake()
    {
        inputField.onSubmit.AddListener((s) => Submit());
    }

    public void SetTicket(Ticket ticket)
    {
        this.ticket = ticket;
    }

    private static char[,] ConvertTo2DCharArray(string inputString, int length, int width)
    {
        string[] inputStringSplitted = inputString.Split("\n");

        char[,] charArray = new char[length, width];

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                string line = inputStringSplitted[i];
                char[] chars = line.ToCharArray();
                charArray[i, j] = chars[j];
            }
        }

        return charArray;
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

    public void Submit()
    {
        if (!inputField.interactable)
            return;

        string submission = inputField.text;

        if (answers.TryGetValue(submission.ToUpper(), out List<Vector2Int> coords))
        {
            foreach (Vector2Int coord in coords)
            {
                CrosswordPieceRow row = pieceRows[coord.x];
                row.crosswordPieces[coord.y].RevealText();
            }

            answers.Remove(submission.ToUpper());
            totalCorrect++;
        }

        inputField.text = "";

        if (totalCorrect == totalSlots)
        {
            ticket.isFixed = true;
            inputField.interactable = false;
            MessageBoxControl.ShowOk("MINIGAME", "You solved the minigame.\nYou can close the window now.");
        }
        else
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
   }

    public void Initialize(string crossword, string answerRaw)
    {
        closeButton.onClick.AddListener(() =>
        {
            if (ticket.isFixed)
                onSubmitOrFinish?.Invoke();
            Close();
        });

        this.answers = new Dictionary<string, List<Vector2Int>>();
        char[,] charArray = ConvertTo2DCharArray(crossword, 11, 11);

        for (int i = 0; i < charArray.GetLength(0); i++)
        {
            for (int j = 0; j < charArray.GetLength(1); j++) 
            {
                pieceRows[i].crosswordPieces[j].SetValue(charArray[i, j]);
            }
        }

        string[] answers = answerRaw.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string answer in answers)
        {
            string[] answerSplitted = answer.Split(" : ");

            List<Vector2Int> coords = ExtractCoordinates(answerSplitted[0]);
            string answerEntry = answerSplitted[1].Trim();
            string definition = answerSplitted[2].Trim();

            clueText.text += definition + "\n\n";

            this.answers.Add(answerEntry.ToUpper(), coords);
        }

        totalSlots = this.answers.Count;
    }

    private List<Vector2Int> ExtractCoordinates(string input)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        // Use regular expression to match coordinates in the input string
        MatchCollection matches = Regex.Matches(input, @"\((\d+),(\d+)\)");

        foreach (Match match in matches)
        {
            // Extract x and y values from the matched groups
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);

            // Create Vector2Int and add to the result list
            result.Add(new Vector2Int(x, y));
        }

        return result;
    }
}


[System.Serializable]
class CrosswordPieceRow
{
    public CrosswordPiece[] crosswordPieces;
}
