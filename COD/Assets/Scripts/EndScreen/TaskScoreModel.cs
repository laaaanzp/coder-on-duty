public class TaskScoreModel
{
    public string name;
    public bool isFixed = false;
    public int score = 0;
    public int totalCorrectAnswers = 0;
    public int totalAnswers = 0;

    public float accuracy
    {
        get => ((float)totalCorrectAnswers / (float)totalAnswers) * 100;
    }
}
