using LMSBackend.Application.DTOs.Dashboard;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IDashboardReadRepository
{
    Task<StudentDashboardDto?> GetStudentAsync(Guid studentId, DashboardQuery query, CancellationToken cancellationToken);
    Task<PageDto<StudentProgressDto>> ListStudentsAsync(StudentDashboardQuery query, CancellationToken cancellationToken);
    Task<StudentTaskProgressDto?> GetTaskAsync(Guid studentId, Guid taskId, CancellationToken cancellationToken);
}
