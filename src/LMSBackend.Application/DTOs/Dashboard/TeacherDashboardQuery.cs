namespace LMSBackend.Application.DTOs.Dashboard;

public sealed class TeacherDashboardQuery
{
    public Guid? SubjectId { get; set; }
    public int SubjectPage { get; set; } = 1;
    public int TaskPage { get; set; } = 1;
    public int AssignmentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
