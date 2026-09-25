using LMSBackend.Application.DTOs.Dashboard;

namespace LMSBackend.Application.Abstractions.Dashboard;

public interface IDashboardService
{
    Task<StudentDashboardDto> GetMineAsync(DashboardQuery query, CancellationToken cancellationToken);
    Task<StudentDashboardDto> GetStudentAsync(Guid studentId, DashboardQuery query, CancellationToken cancellationToken);
    Task<PageDto<StudentProgressDto>> ListStudentsAsync(StudentDashboardQuery query, CancellationToken cancellationToken);
    Task<StudentTaskProgressDto> GetTaskAsync(Guid studentId, Guid taskId, CancellationToken cancellationToken);
}
