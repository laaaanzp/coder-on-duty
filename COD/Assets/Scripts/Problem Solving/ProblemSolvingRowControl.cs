using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProblemSolvingRowControl : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject spaceNodePrefab;
    [SerializeField] private GameObject slotNodePrefab;
    [SerializeField] private RectTransform rectTransform;


    public void AddToken(string token)
    {
        if (token.StartsWith("NODE_SLOT:"))
        {
            GameObject slotNodeObj = Instantiate(slotNodePrefab, transform);
            SlotNode slotNode = slotNodeObj.GetComponent<SlotNode>();

            string answer = token.Replace("NODE_SLOT:", "");
  
            slotNode.correctAnswer = answer;
        }
        else
        {
            token = CodeHighlighter.CSSyntaxHighlight(token);
            GameObject nodeObj = Instantiate(nodePrefab, transform);
            TextMeshProUGUI nodeText = nodeObj.GetComponent<TextMeshProUGUI>();
            nodeText.text = token;

            if (token.EndsWith(' '))
            {
                Instantiate(spaceNodePrefab, transform);
            }
        }

        RebuildLayout();
    }

    private void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
