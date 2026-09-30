namespace LMSBackend.Application.DTOs.Groups;

public sealed record UpdateGroupStudentsRequest
{
    public Guid[] StudentIds { get; init; } = [];
}
