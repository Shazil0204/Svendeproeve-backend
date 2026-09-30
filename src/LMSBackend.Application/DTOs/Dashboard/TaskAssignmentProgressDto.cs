using LMSBackend.Application.DTOs.Submissions;

namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record TaskAssignmentProgressDto(DashboardTaskDto Assignment,
    SubmissionDto? Submission, IReadOnlyList<FeedbackDto> Feedback);
