namespace LMSBackend.Application.DTOs.Dashboard;

public sealed class StudentDashboardQuery
{
    public Guid? GroupId { get; set; }
    public Guid? SubjectId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
