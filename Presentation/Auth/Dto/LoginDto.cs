using System.ComponentModel.DataAnnotations;
using Presentation.Validation;

namespace Presentation.Auth.Dto;

public record LoginDto(
    [Required]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Username must start with a letter and contain only letters and numbers")]
    [property: UniqueUsername]
    string Username);