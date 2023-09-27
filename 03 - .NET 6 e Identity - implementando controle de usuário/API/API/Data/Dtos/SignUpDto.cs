using System.ComponentModel.DataAnnotations;

namespace API.Data.Dtos;

public class SignUpDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
}
