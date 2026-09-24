namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record SubmitQuizRequest(
    IEnumerable<SubmitQuizAnswerRequest> Answers);