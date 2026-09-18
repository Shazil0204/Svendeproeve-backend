namespace LMSBackend.Domain.Entities.Quizzes;

public class QuizQuestion
{
    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public string QuestionText { get; private set; } = string.Empty;
    public Quiz Quiz { get; private set; } = null!;

    private QuizQuestion() { }

    public QuizQuestion(Guid quizId, string questionText)
    {
        Id = Guid.NewGuid();
        QuizId = quizId;
        QuestionText = questionText;
    }

    public void Update(string questionText)
    {
        QuestionText = questionText;
    }
}