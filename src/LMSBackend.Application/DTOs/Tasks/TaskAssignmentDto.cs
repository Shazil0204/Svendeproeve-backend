using LMSBackend.Domain.Enums.Tasks;

namespace LMSBackend.Application.DTOs.Tasks;

public sealed record TaskAssignmentDto(Guid AssignmentId, TaskDto Task,
    string RecipientType, Guid RecipientId, string RecipientName,
    DateTimeOffset? Deadline, StudentTaskStatus Status, DateTimeOffset AssignedAt);

