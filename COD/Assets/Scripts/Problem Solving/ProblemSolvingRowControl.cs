using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProblemSolvingRowControl : MonoBehaviour
{
    public GameObject keywordNode;
    public GameObject identifierNode;
    public GameObject stringNode;
    public GameObject numericNode;
    public GameObject spaceNode;
    public GameObject tabNode;
    public GameObject blankNode;

    private RectTransform rectTransform;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AddKeyword(string text)
    {
        GameObject node = Instantiate(keywordNode, transform);
        node.GetComponent<TextMeshProUGUI>().text = text;

        RebuildLayout();
    }

    public void AddIdentifier(string text)
    {
        GameObject node = Instantiate(identifierNode, transform);
        node.GetComponent<TextMeshProUGUI>().text = text;

        RebuildLayout();
    }

    public void AddString(string text)
    {
        GameObject node = Instantiate(stringNode, transform);
        node.GetComponent<TextMeshProUGUI>().text = text;

        RebuildLayout();
    }

    public void AddNumeric(string text)
    {
        GameObject node = Instantiate(numericNode, transform);
        node.GetComponent<TextMeshProUGUI>().text = text;

        RebuildLayout();
    }

    public void AddSpace()
    {
        Instantiate(spaceNode, transform);

        RebuildLayout();
    }

    public void AddTab()
    {
        Instantiate(tabNode, transform);

        RebuildLayout();
    }

    private void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
