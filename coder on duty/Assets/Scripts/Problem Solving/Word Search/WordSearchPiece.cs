using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordSearchPiece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;

    private bool isHighlighted;
    private Color highlightedColor = new Color(1.0f, 0.647f, 0.0f, 1.0f);
    private Color selectedColor = Color.cyan;

    public string value
    {
        get => text.text;
    }

    private bool interactable;

    private static List<WordSearchPiece> instances;
    private static WordSearchPiece first;
    private static WordSearchPiece last;
    private static HashSet<WordSearchPiece> selected;


    void Awake()
    {
        instances = new List<WordSearchPiece>();
        selected = new HashSet<WordSearchPiece>();
        first = last = null;
        interactable = true;
    }

    void Start()
    {
        instances.Add(this);
    }


    private char GetRandomLetter()
    {
        int randomIndex = Random.Range(0, 26);

        char randomLetter = (char)('A' + randomIndex);

        return randomLetter;
    }

    public void SetValue(char value)
    {
        if (value != '-')
            text.text = value.ToString();

        else
            text.text = GetRandomLetter().ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable)
            return;

        if (first == null)
            return;

        if (last != null)
        {
            Vector2Int firstCoords = first.GetCoords();
            Vector2Int lastCoords = last.GetCoords();
            Vector2Int selfCoords = GetCoords();

            if (firstCoords.x == lastCoords.x)
            {
                if (selfCoords.x != firstCoords.x)
                    return;
            }
            else if (firstCoords.y == lastCoords.y)
            {
                if (selfCoords.y != firstCoords.y)
                    return;
            }
        }
        else
        {
            if (first != this)
                last = this;
        }

        WordSearch.SetCensorRaycastPosition(transform.position);
        selected.Add(this);

        image.color = selectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable)
            return;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        first = this;
        last = null;

        selected.Add(this);

        image.color = selectedColor;

        Vector2Int coords = GetCoords();
        
        foreach (WordSearchPiece piece in instances)
        {
            if (piece == this)
                continue;

            Vector2Int otherCoords = piece.GetCoords();
            piece.SetInteractable(otherCoords.x == coords.x ||  otherCoords.y == coords.y);
        }
    }

    public Vector2Int GetCoords()
    {
        int x = transform.GetSiblingIndex();
        int y = transform.parent.GetSiblingIndex();

        return new Vector2Int(x, y);
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            WordSearch.SetCensorRaycastPosition(new Vector3(9999, 9999, 9999));
            interactable = true;

            if (selected.Count >= 2)
            {
                SortSelected();
            }

            first = last = null;

            string result = "";

            foreach (WordSearchPiece piece in selected)
                result += piece.value;

            if (result != "")
            {
                if (WordSearch.Submit(result))
                {
                    foreach (WordSearchPiece piece in selected)
                    {
                        result += piece.value;
                        piece.isHighlighted = true;
                    }
                }
            }

            selected.Clear();
            image.color = isHighlighted ? highlightedColor : Color.white;
        }
    }

    public void SortSelected()
    {
        List<WordSearchPiece> sortedList = selected.ToList();

        if (first.GetCoords().y == last.GetCoords().y)
            sortedList.Sort((a, b) => a.GetCoords().x.CompareTo(b.GetCoords().x));

        else
            sortedList.Sort((a, b) => a.GetCoords().y.CompareTo(b.GetCoords().y));

        selected = new HashSet<WordSearchPiece>(sortedList);
    }
}
