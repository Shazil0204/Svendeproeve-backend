namespace LMSBackend.Domain.Entities.Quizzes;

public class QuizAnswer
{
    public Guid Id { get; private set; }
    public Guid QuizStudentId { get; private set; }
    public Guid QuizQuestionId { get; private set; }
    public Guid SelectedAnswerOptionId { get; private set; }
    public QuizStudent QuizStudent { get; private set; } = null!;
    public QuizQuestion QuizQuestion { get; private set; } = null!;
    public QuizAnswerOption SelectedAnswerOption { get; private set; } = null!;

    private QuizAnswer() { }

    public QuizAnswer(
        Guid quizStudentId,
        Guid quizQuestionId,
        Guid selectedAnswerOptionId)
    {
        Id = Guid.NewGuid();
        QuizStudentId = quizStudentId;
        QuizQuestionId = quizQuestionId;
        SelectedAnswerOptionId = selectedAnswerOptionId;
    }
}