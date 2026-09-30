using System.Security.Claims;
using LMSBackend.Application.Abstractions.Groups;
using LMSBackend.Application.DTOs.Groups;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groups;
    
    public GroupsController(IGroupService groups)
    {
        _groups = groups;
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Teacher)}, {nameof(UserRole.Student)}")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        if (User.IsInRole(nameof(UserRole.Teacher)))
            return Ok(await _groups.ListAsync(cancellationToken));
        if (!TryGetStudentId(out var studentId))
            return Forbid();
        return Ok(await _groups.ListForStudentAsync(studentId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Teacher)}, {nameof(UserRole.Student)}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        if (User.IsInRole(nameof(UserRole.Teacher)))
            return Ok(await _groups.GetAsync(id, cancellationToken));
        if (!TryGetStudentId(out var studentId))
            return Forbid();
        return Ok(await _groups.GetForStudentAsync(id, studentId, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<IActionResult> Create(GroupRequest request, CancellationToken cancellationToken)
    {
        GroupDto group = await _groups.CreateAsync(request.Name, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = group.Id }, group);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<ActionResult<GroupDto>> Rename(Guid id, UpdateGroupNameRequest request, CancellationToken cancellationToken)
    {
        GroupDto group = await _groups.RenameAsync(id, request.Name, cancellationToken);
        return Ok(group);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _groups.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<IActionResult> AddStudents(Guid id, [FromBody] UpdateGroupStudentsRequest request, CancellationToken cancellationToken)
    {
        await _groups.AddStudentsAsync(id, request.StudentIds, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/members")]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    public async Task<IActionResult> RemoveStudents(Guid id, [FromBody] UpdateGroupStudentsRequest request, CancellationToken cancellationToken)
    {
        await _groups.RemoveStudentsAsync(id, request.StudentIds, cancellationToken);
        return NoContent();
    }

    private bool TryGetStudentId(out Guid studentId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out studentId)
        && studentId != Guid.Empty;
}
