using System.Security.Claims;
using LMSBackend.API.Models;
using LMSBackend.Application.Abstractions.Submissions;
using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Application.Abstractions.Tasks;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/task-assignments")]
public sealed class TaskAssignmentsController : ControllerBase
{
    private readonly ITaskService _tasks;
    private readonly ISubmissionService _submissions;
    public TaskAssignmentsController(ITaskService tasks, ISubmissionService submissions)
    {
        _tasks = tasks;
        _submissions = submissions;
    }

    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<IReadOnlyList<TaskAssignmentDto>>> ListMine(CancellationToken cancellationToken)
    {
        if (!TryGetStudentId(out Guid studentId))
            return Forbid();
        return Ok(await _tasks.ListAssignmentsForStudentAsync(studentId, cancellationToken));
    }

    [HttpGet("{assignmentId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
    public async Task<ActionResult<TaskAssignmentDto>> Get(Guid assignmentId, CancellationToken cancellationToken)
    {
        if (User.IsInRole(nameof(UserRole.Teacher)))
            return Ok(await _tasks.GetAssignmentAsync(assignmentId, null, cancellationToken));
        if (!TryGetStudentId(out Guid studentId))
            return Forbid();
        return Ok(await _tasks.GetAssignmentAsync(assignmentId, studentId, cancellationToken));
    }

    [HttpPost("{assignmentId:guid}/submission")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ISubmissionFileStore.MaxFileBytes + 65536)]
    [RequestFormLimits(MultipartBodyLengthLimit = ISubmissionFileStore.MaxFileBytes + 65536)]
    public async Task<ActionResult<SubmissionDto>> Submit(Guid assignmentId, [FromForm] SubmitTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (Request.Form.Files.Count > 1 || Request.Form.Files.Any(f => f.Name != "file" && f.Name != "File"))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Only one file named 'file' is accepted." });
        await using Stream? stream = request.File?.OpenReadStream();
        var submission = await _submissions.SubmitAsync(assignmentId, request.Comment, stream,
            request.File?.FileName, cancellationToken);
        return CreatedAtAction(nameof(GetSubmission), new { assignmentId }, submission);
    }

    [HttpGet("{assignmentId:guid}/submission")]
    [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
    public async Task<ActionResult<SubmissionDto>> GetSubmission(Guid assignmentId, CancellationToken cancellationToken) =>
        Ok(await _submissions.GetAsync(assignmentId, User.IsInRole(nameof(UserRole.Teacher)), cancellationToken));

    [HttpGet("{assignmentId:guid}/submission/file")]
    [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
    public async Task<IActionResult> DownloadSubmission(Guid assignmentId, CancellationToken cancellationToken)
    {
        var file = await _submissions.DownloadAsync(assignmentId, User.IsInRole(nameof(UserRole.Teacher)), cancellationToken);
        return File(file.Content, "application/zip", file.FileName);
    }

    [HttpPatch("{assignmentId:guid}/submission/status")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<ActionResult<SubmissionDto>> ReviewSubmission(Guid assignmentId,
        ReviewSubmissionRequest request, CancellationToken cancellationToken) =>
        Ok(await _submissions.ReviewAsync(assignmentId, request, cancellationToken));

    [HttpDelete("{assignmentId:guid}")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<IActionResult> RemoveAssignment(Guid assignmentId, CancellationToken cancellationToken)
    {
        await _submissions.RemoveAssignmentAsync(assignmentId, cancellationToken);
        return NoContent();
    }

    [HttpPost("/api/v1/students/{studentId:guid}/tasks/{taskId:guid}/feedback")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<ActionResult<FeedbackDto>> AddFeedback(Guid studentId, Guid taskId,
        CreateFeedbackRequest request, CancellationToken cancellationToken)
    {
        var feedback = await _submissions.AddFeedbackAsync(studentId, taskId, request, cancellationToken);
        return CreatedAtAction(nameof(GetFeedback), new { studentId, taskId }, feedback);
    }

    [HttpGet("/api/v1/students/{studentId:guid}/tasks/{taskId:guid}/feedback")]
    [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
    public async Task<ActionResult<IReadOnlyList<FeedbackDto>>> GetFeedback(Guid studentId, Guid taskId,
        CancellationToken cancellationToken) =>
        Ok(await _submissions.GetFeedbackAsync(studentId, taskId, User.IsInRole(nameof(UserRole.Teacher)), cancellationToken));

    private bool TryGetStudentId(out Guid studentId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out studentId)
        && studentId != Guid.Empty;
}
