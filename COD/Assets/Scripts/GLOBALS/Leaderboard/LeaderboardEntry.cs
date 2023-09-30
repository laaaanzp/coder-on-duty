using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;
    
    
    public void SetValues(string name, string value)
    {
        nameText.text = name;
        valueText.text = value;
    }
}
