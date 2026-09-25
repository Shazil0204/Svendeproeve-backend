namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record ProgressSummaryDto(TaskProgressSummaryDto Tasks, QuizProgressSummaryDto Quizzes);
