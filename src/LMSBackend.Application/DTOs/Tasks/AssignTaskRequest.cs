namespace LMSBackend.Application.DTOs.Tasks;

public sealed record AssignTaskRequest
{
    public IReadOnlyCollection<Guid>? StudentIds { get; init; }
    public IReadOnlyCollection<Guid>? GroupIds { get; init; }
}

