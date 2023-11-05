using System.ComponentModel.DataAnnotations;
using API.Data.Validation;

namespace API.Data.Dtos;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [IsBoolean(IsOptional = true)]
    public bool? IsPersistent { get; set; }
}