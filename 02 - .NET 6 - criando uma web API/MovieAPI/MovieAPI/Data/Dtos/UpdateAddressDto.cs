using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class UpdateAddressDto
{
    public string? Street { get; set; } = null!;

    public string? Number { get; set; } = null!;
}