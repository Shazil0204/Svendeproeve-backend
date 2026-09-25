using LMSBackend.Application.Abstractions.Dashboard;
using LMSBackend.Application.DTOs.Dashboard;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public sealed class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<StudentDashboardDto>> GetMine([FromQuery] DashboardQuery query,
        CancellationToken cancellationToken) =>
        Ok(await dashboard.GetMineAsync(query, cancellationToken));

    [HttpGet("students")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<ActionResult<PageDto<StudentProgressDto>>> ListStudents(
        [FromQuery] StudentDashboardQuery query, CancellationToken cancellationToken) =>
        Ok(await dashboard.ListStudentsAsync(query, cancellationToken));

    [HttpGet("students/{studentId:guid}")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<ActionResult<StudentDashboardDto>> GetStudent(Guid studentId,
        [FromQuery] DashboardQuery query, CancellationToken cancellationToken) =>
        Ok(await dashboard.GetStudentAsync(studentId, query, cancellationToken));
}
