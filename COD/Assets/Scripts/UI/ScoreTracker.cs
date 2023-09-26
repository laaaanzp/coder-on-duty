using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static int score = 0;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private static ScoreTracker instance;


    void Awake()
    {
        score = 0;
        instance = this;
        UpdateTotalScoreDisplay();
    }

    public static void AddScore(int additionalScore)
    {
        score += additionalScore;
        instance.UpdateTotalScoreDisplay();
    }

    private void UpdateTotalScoreDisplay()
    {
        totalScoreText.text = $"<b>Score:</b> {score}";
    }
}
