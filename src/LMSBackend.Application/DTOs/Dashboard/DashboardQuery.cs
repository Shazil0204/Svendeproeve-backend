namespace LMSBackend.Application.DTOs.Dashboard;

public sealed class DashboardQuery
{
    public Guid? SubjectId { get; set; }
    public int TaskPage { get; set; } = 1;
    public int QuizPage { get; set; } = 1;
    public int FeedbackPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
