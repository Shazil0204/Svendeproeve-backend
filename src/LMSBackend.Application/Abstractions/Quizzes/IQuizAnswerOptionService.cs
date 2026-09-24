using LMSBackend.Application.DTOs.Quizzes;

namespace LMSBackend.Application.Abstractions.Quizzes;

public interface IQuizAnswerOptionService
{
    Task AddAnswerOptionAsync(
        Guid questionId,
        CreateQuizAnswerOptionRequest request);

    Task UpdateAnswerOptionAsync(
        Guid answerOptionId,
        UpdateQuizAnswerOptionRequest request);

    Task<IEnumerable<QuizAnswerOptionResponse>>
        GetAnswerOptionsByQuestionIdAsync(Guid questionId);

    Task DeleteAnswerOptionAsync(Guid answerOptionId);
}