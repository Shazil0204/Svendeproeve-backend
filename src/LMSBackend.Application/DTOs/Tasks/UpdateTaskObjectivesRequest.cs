using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Tasks;

public sealed record UpdateTaskObjectivesRequest
{
    [Required]
    public IReadOnlyCollection<Guid> ObjectiveIds { get; init; } = null!;
}
