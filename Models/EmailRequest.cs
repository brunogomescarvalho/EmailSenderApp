using System.ComponentModel.DataAnnotations;

namespace EmailSenderApp.Models;

public record EmailRequest
{
    [Required, EmailAddress]
    public string To { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string From { get; init; } = string.Empty;

    [Required, MaxLength(100), MinLength(5)]
    public string Subject { get; init; } = string.Empty;

    [Required, MaxLength(5000), MinLength(10)]
    public string Message { get; init; } = string.Empty;

    [Required, MaxLength(50), MinLength(2)]
    public string Name { get; init; } = string.Empty;

    public bool IsValid(out List<ValidationResult> results)
    {
        var context = new ValidationContext(this);
        results = [];
        return Validator.TryValidateObject(this, context, results, true);
    }
}
