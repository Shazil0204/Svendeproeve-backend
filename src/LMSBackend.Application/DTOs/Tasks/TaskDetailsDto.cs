namespace LMSBackend.Application.DTOs.Tasks;

public sealed record TaskDetailsDto(TaskDto Task, IReadOnlyList<TaskObjectiveDto> Objectives,
    IReadOnlyList<TaskAssignmentDto> Assignments);

