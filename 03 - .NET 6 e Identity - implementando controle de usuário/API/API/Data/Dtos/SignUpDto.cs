using System.ComponentModel.DataAnnotations;
using API.Data.Enums;

namespace API.Data.Dtos;

public class SignUpDto
{
    [Required]
    public string Name { get; set; } = null!;

    // Role
    [Required]
    [EnumDataType(typeof(UserRoleEnum))]
    public string Role { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
}
