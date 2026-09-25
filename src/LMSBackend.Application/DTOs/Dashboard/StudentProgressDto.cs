namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record StudentProgressDto(Guid StudentId, string StudentName, ProgressSummaryDto Summary);
