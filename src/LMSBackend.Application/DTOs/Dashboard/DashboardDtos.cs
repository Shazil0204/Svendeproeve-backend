using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Application.DTOs.Tasks;

namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record PageDto<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed class DashboardQuery
{
    public Guid? SubjectId { get; set; }
    public int TaskPage { get; set; } = 1;
    public int QuizPage { get; set; } = 1;
    public int FeedbackPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public sealed class StudentDashboardQuery
{
    public Guid? GroupId { get; set; }
    public Guid? SubjectId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed record TaskProgressSummaryDto(int Assigned, int Approved, int AwaitingReview,
    int Rejected, int NotSubmitted, decimal CompletionPercentage);
public sealed record QuizProgressSummaryDto(int Assigned, int Completed, int Passed, int Failed,
    decimal CompletionPercentage);
public sealed record ProgressSummaryDto(TaskProgressSummaryDto Tasks, QuizProgressSummaryDto Quizzes);
public sealed record StudentProgressDto(Guid StudentId, string StudentName, ProgressSummaryDto Summary);
public sealed record StudentDashboardDto(Guid StudentId, string StudentName, ProgressSummaryDto Summary,
    PageDto<DashboardTaskDto> Tasks, PageDto<DashboardQuizDto> Quizzes,
    PageDto<DashboardFeedbackDto> RecentFeedback);

public sealed record DashboardTaskDto(Guid AssignmentId, Guid TaskId, string Title, Guid SubjectId,
    string RecipientType, Guid? GroupId, string Status, DateTimeOffset AssignedAt,
    DateTimeOffset? Deadline, DateTimeOffset? SubmittedAt, int FeedbackCount);
public sealed record DashboardQuizDto(Guid AssignmentId, Guid QuizId, string Title, Guid SubjectId,
    string Status, decimal? ScorePercentage, bool? Passed, DateTimeOffset AssignedAt,
    DateTimeOffset? CompletedAt);
public sealed record DashboardFeedbackDto(Guid Id, Guid TaskId, string TaskTitle, Guid AssignmentId,
    Guid SubmissionId, string Text, DateTimeOffset CreatedAt, Guid TeacherId, string TeacherName);

public sealed record StudentTaskProgressDto(Guid StudentId, TaskDto Task,
    IReadOnlyList<TaskObjectiveDto> Objectives, IReadOnlyList<TaskAssignmentProgressDto> Assignments);
public sealed record TaskAssignmentProgressDto(DashboardTaskDto Assignment,
    SubmissionDto? Submission, IReadOnlyList<FeedbackDto> Feedback);
