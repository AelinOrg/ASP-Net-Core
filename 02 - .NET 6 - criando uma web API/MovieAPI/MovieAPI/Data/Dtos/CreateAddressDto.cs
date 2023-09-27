using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class CreateAddressDto
{
    [Required(ErrorMessage = "Street is required")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "Number is required")]
    public string Number { get; set; } = null!;
}