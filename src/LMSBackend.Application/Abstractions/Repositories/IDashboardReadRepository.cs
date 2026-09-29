using LMSBackend.Application.DTOs.Dashboard;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IDashboardReadRepository
{
    Task<TeacherDashboardDto?> GetTeacherAsync(Guid teacherId, TeacherDashboardQuery query, CancellationToken cancellationToken);
    Task<StudentDashboardDto?> GetStudentAsync(Guid studentId, DashboardQuery query, CancellationToken cancellationToken);
    Task<PageDto<StudentProgressDto>> ListStudentsAsync(StudentDashboardQuery query, CancellationToken cancellationToken);
    Task<StudentTaskProgressDto?> GetTaskAsync(Guid studentId, Guid taskId, CancellationToken cancellationToken);
}
