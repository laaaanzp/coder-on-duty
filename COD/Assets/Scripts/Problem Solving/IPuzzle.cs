using System;

public interface IPuzzle
{
    Action onSubmitOrFinish { get; set; }
    int totalCorrect { get; set; }
    int timeRemaining { get; set; }
    int totalSlots { get; set; }
    TaskScoreModel taskScoreModel { get; set; }
    void SetTicket(Ticket ticket);
    void Open();
    void Close();
    void CheckAnswers();
}
