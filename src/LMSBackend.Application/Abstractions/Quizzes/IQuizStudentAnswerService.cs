using LMSBackend.Application.DTOs.Quizzes;

namespace LMSBackend.Application.Abstractions.Quizzes;

public interface IQuizStudentAnswerService
{
    Task AddStudentToQuizAsync(
        Guid quizId,
        AddQuizStudentRequest request);

    Task RemoveStudentFromQuizAsync(
        Guid quizId,
        Guid studentId);

    Task<IEnumerable<QuizStudentResponse>>
        GetQuizStudentsAsync(Guid quizId);

    Task<StudentQuizResponse> GetStudentQuizAsync(
        Guid quizId,
        Guid studentId);

    Task SubmitQuizAsync(
        Guid quizId,
        Guid studentId,
        SubmitQuizRequest request);

    Task<IEnumerable<QuizAnswerResponse>>
        GetStudentAnswersAsync(
            Guid quizId,
            Guid studentId);
}