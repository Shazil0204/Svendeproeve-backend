namespace LMSBackend.Domain.Entities.Quizzes;

public class QuizAnswerOption
{
    public Guid Id { get; private set; }
    public Guid QuizQuestionId { get; private set; }
    public string AnswerText { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }
    public QuizQuestion QuizQuestion { get; private set; } = null!;

    private QuizAnswerOption() { }

    public QuizAnswerOption(
        Guid quizQuestionId,
        string answerText,
        bool isCorrect)
    {
        Id = Guid.NewGuid();
        QuizQuestionId = quizQuestionId;
        AnswerText = answerText;
        IsCorrect = isCorrect;
    }

    public void Update(
        string answerText,
        bool isCorrect)
    {
        AnswerText = answerText;
        IsCorrect = isCorrect;
    }
}