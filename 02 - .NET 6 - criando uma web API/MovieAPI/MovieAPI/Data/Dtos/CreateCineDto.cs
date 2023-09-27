using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class CreateCineDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "AddressId is required")]
    public int AddressId { get; set; }
}