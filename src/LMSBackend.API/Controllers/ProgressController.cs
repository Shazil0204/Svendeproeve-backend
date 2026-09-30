using LMSBackend.Application.Abstractions.Dashboard;
using LMSBackend.Application.DTOs.Dashboard;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/progress")]
[Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
public sealed class ProgressController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet("students/{studentId:guid}/tasks/{taskId:guid}")]
    public async Task<ActionResult<StudentTaskProgressDto>> GetTask(Guid studentId, Guid taskId,
        CancellationToken cancellationToken) =>
        Ok(await dashboard.GetTaskAsync(studentId, taskId, cancellationToken));
}
