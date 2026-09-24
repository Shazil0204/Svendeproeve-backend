using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IQuizAnswerOptionRepository
{
    Task AddQuizAnswerOption(QuizAnswerOption answerOption);

    Task<QuizAnswerOption?> GetQuizAnswerOptionById(Guid answerOptionId);

    Task<IEnumerable<QuizAnswerOption>> GetQuizAnswerOptionsByQuestionId(
        Guid quizQuestionId);

    Task<int> GetAnswerOptionCountByQuestionId(Guid quizQuestionId);

    Task<bool> HasCorrectAnswer(Guid quizQuestionId);

    void DeleteQuizAnswerOption(QuizAnswerOption answerOption);
}