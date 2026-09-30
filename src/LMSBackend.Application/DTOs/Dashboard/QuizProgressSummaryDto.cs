namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record QuizProgressSummaryDto(int Assigned, int Completed, int Passed, int Failed,
    decimal CompletionPercentage);
