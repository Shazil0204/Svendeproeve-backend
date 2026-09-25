namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record StudentDashboardDto(Guid StudentId, string StudentName, ProgressSummaryDto Summary,
    PageDto<DashboardTaskDto> Tasks, PageDto<DashboardQuizDto> Quizzes,
    PageDto<DashboardFeedbackDto> RecentFeedback);
