using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Quizzes;
using LMSBackend.Domain.Exceptions;
using LMSBackend.Domain.ValueObjects.Progression;

namespace LMSBackend.Domain.Entities.Quizzes;

public class QuizStudent
{
    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public Percentage? ScorePercentage { get; private set; }
    public bool? Passed { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public QuizStatus Status { get; private set; }
    public Quiz Quiz { get; private set; } = null!;
    public User Student { get; private set; } = null!;

    private QuizStudent() { }

    public QuizStudent(Guid quizId, Guid studentId)
    {
        Id = Guid.NewGuid();
        QuizId = quizId;
        StudentId = studentId;
        AssignedAt = DateTimeOffset.UtcNow;
        Status = QuizStatus.Available;
    }

    public void CompleteQuiz(Percentage scorePercentage, bool passed, QuizStatus status)
    {
        if (Status != QuizStatus.Available)
            throw new DomainValidationException("Quiz is not available for completion.");

        ScorePercentage = scorePercentage;
        Passed = passed;
        CompletedAt = DateTimeOffset.UtcNow;
        Status = status;
    }
}