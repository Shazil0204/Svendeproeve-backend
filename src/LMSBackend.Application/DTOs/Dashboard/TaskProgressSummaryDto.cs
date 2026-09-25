namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record TaskProgressSummaryDto(int Assigned, int Approved, int AwaitingReview,
    int Rejected, int NotSubmitted, decimal CompletionPercentage);
