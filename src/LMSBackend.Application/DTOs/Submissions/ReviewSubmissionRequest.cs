using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Submissions;

public sealed record ReviewSubmissionRequest
{
    [Required]
    public string Status { get; init; } = string.Empty;
}

