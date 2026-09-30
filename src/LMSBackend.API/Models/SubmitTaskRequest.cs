using System.ComponentModel.DataAnnotations;

namespace LMSBackend.API.Models;

public sealed class SubmitTaskRequest
{
    [Required(AllowEmptyStrings = true), StringLength(5000)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Comment { get; init; } = null!;
    public IFormFile? File { get; init; }
}

