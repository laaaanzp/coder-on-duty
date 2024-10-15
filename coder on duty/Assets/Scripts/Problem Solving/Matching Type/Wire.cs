using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private string value;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    public bool isRight;
    public bool isConnected;

    private Wire connectedTo;

    private GameObject line;

    private static Wire hoverWire;
    private static List<Wire> leftWires;
    private static List<Wire> rightWires;

    
    void Awake()
    {
        leftWires = new List<Wire>();
        rightWires = new List<Wire>();
    }

    void Start()
    {
        if (isRight)
            rightWires.Add(this);
        else
            leftWires.Add(this);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetValue(string value)
    {
        this.value = value;
    }

    public static bool Check()
    {
        foreach (Wire leftWire in leftWires)
        {
            if (!leftWire.CheckAnswer())
                return false;
        }

        return true;
    }

    public bool CheckAnswer()
    {
        if (connectedTo == null)
            return false;

        return connectedTo.value == value;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isRight)
            return;

        UpdateLine(eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isRight)
            return;

        if (connectedTo != null)
        {
            connectedTo.isConnected = false;
            connectedTo = null;
        }

        Destroy(line);
        line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform.parent.parent.parent);
        UpdateLine(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoverWire == null)
        {
            Destroy(line);
            return;
        }

        else if (hoverWire.isConnected)
        {
            foreach (Wire leftWire in leftWires)
            {
                if (leftWire.connectedTo == hoverWire)
                {
                    leftWire.connectedTo = null;
                    Destroy(leftWire.line);
                    hoverWire.isConnected = false;
                }
            }
        }

        if (!Equals(hoverWire))
        {
            UpdateLine(hoverWire.transform.position);
            connectedTo = hoverWire;
            hoverWire.isConnected = true;

            if (Check())
                MatchingType.OnFinish();
        }
        else
        {
            Destroy(line);
        }
    }

    private void UpdateLine(Vector3 position)
    {
        Vector3 direction = position - transform.position;

        line.transform.right = direction;

        line.transform.localScale = new Vector3(direction.magnitude, 1f, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isRight)
            return;

        hoverWire = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverWire = null;
    }
}
