using LMSBackend.Application.Abstractions.Tasks;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = nameof(UserRole.Teacher))]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> List(CancellationToken cancellationToken) =>
        Ok(await _tasks.ListAsync(cancellationToken));

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<TaskDetailsDto>> Get(Guid taskId, CancellationToken cancellationToken) =>
        Ok(await _tasks.GetAsync(taskId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(SaveTaskRequest request, CancellationToken cancellationToken)
    {
        TaskDto task = await _tasks.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { taskId = task.Id }, task);
    }

    [HttpPatch("{taskId:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid taskId, SaveTaskRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _tasks.UpdateAsync(taskId, request, cancellationToken));

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid taskId, CancellationToken cancellationToken)
    {
        await _tasks.DeleteAsync(taskId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{taskId:guid}/assignments")]
    public async Task<ActionResult<IReadOnlyList<TaskAssignmentDto>>> Assign(Guid taskId,
        AssignTaskRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TaskAssignmentDto> assignments = await _tasks.AssignAsync(taskId, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { taskId }, assignments);
    }

    [HttpPut("{taskId:guid}/objectives")]
    public async Task<ActionResult<IReadOnlyList<TaskObjectiveDto>>> SetObjectives(Guid taskId,
        UpdateTaskObjectivesRequest request, CancellationToken cancellationToken) =>
        Ok(await _tasks.SetObjectivesAsync(taskId, request, cancellationToken));
}

