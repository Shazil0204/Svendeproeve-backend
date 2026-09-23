using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Submissions;

public sealed record CreateFeedbackRequest
{
    [Required, StringLength(5000)]
    public string Text { get; init; } = string.Empty;
    public Guid? AssignmentId { get; init; }
}
