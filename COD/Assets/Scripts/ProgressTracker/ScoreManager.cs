using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private static ScoreManager instance;

    public static int totalCorrect;
    public static int totalSlots;

    private static LTDescr currentDescr;

    public static float accuracy
    {
        get => (totalCorrect / totalSlots) * 100;
    }

    void Awake()
    {
        instance = this;
        score = 0;
        totalCorrect = 0;
        totalSlots = 0;
    }

    public static void AddScore(int additionalScore)
    {
        int newScore = score + additionalScore;

        if (currentDescr != null)
        {
            LeanTween.cancel(currentDescr.uniqueId);
        }

        currentDescr = LeanTween.value(score, newScore, 1f).setOnUpdate(score =>
        {
            instance.UpdateScoreDisplay((int)score);
        });

        score = newScore;
    }

    private void UpdateScoreDisplay(int score)
    {
        totalScoreText.text = $"<b>Score:</b> {score}";
    }
}
