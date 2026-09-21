namespace LMSBackend.Application.DTOs.Groups;

public sealed class GroupStudentsRequest
{
    public Guid[] StudentIds { get; init; } = [];
}
