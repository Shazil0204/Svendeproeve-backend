using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Dashboard;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Dashboard;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.Services.Dashboard;

public sealed class DashboardService(IDashboardReadRepository repository, ICurrentUserService currentUser)
    : IDashboardService
{
    public Task<StudentDashboardDto> GetMineAsync(DashboardQuery query, CancellationToken cancellationToken)
    {
        Guid studentId = UserId;
        if (currentUser.Role != nameof(UserRole.Student))
            throw new NotFoundException("Student dashboard not found.");
        return GetStudentAsync(studentId, query, cancellationToken);
    }

    public async Task<StudentDashboardDto> GetStudentAsync(Guid studentId, DashboardQuery query,
        CancellationToken cancellationToken)
    {
        EnsureCanRead(studentId);
        ValidatePage(query.TaskPage, query.PageSize);
        ValidatePage(query.QuizPage, query.PageSize);
        ValidatePage(query.FeedbackPage, query.PageSize);
        return await repository.GetStudentAsync(studentId, query, cancellationToken)
            ?? throw new NotFoundException("Student dashboard not found.");
    }

    public Task<PageDto<StudentProgressDto>> ListStudentsAsync(StudentDashboardQuery query,
        CancellationToken cancellationToken)
    {
        _ = UserId;
        if (currentUser.Role != nameof(UserRole.Teacher))
            throw new NotFoundException("Student dashboards not found.");
        ValidatePage(query.Page, query.PageSize);
        return repository.ListStudentsAsync(query, cancellationToken);
    }

    public async Task<StudentTaskProgressDto> GetTaskAsync(Guid studentId, Guid taskId,
        CancellationToken cancellationToken)
    {
        EnsureCanRead(studentId);
        return await repository.GetTaskAsync(studentId, taskId, cancellationToken)
            ?? throw new NotFoundException("Student task not found.");
    }

    private Guid UserId => currentUser.UserId is { } id && id != Guid.Empty
        ? id : throw new UnauthorizedAccessException("User is not authenticated.");

    private void EnsureCanRead(Guid studentId)
    {
        Guid userId = UserId;
        // Teachers currently have school-wide read access, matching tasks and groups.
        if (currentUser.Role == nameof(UserRole.Teacher)) return;
        if (currentUser.Role != nameof(UserRole.Student) || userId != studentId)
            throw new NotFoundException("Student progress not found.");
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new ValidationException("Page must be positive and pageSize must be between 1 and 100, with an offset no greater than 2147483647.");
    }
}
