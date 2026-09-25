using LMSBackend.Application.DTOs.Tasks;

namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record StudentTaskProgressDto(Guid StudentId, TaskDto Task,
    IReadOnlyList<TaskObjectiveDto> Objectives, IReadOnlyList<TaskAssignmentProgressDto> Assignments);
