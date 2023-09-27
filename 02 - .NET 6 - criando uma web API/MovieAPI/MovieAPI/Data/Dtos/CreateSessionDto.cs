using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class CreateSessionDto
{
    [Required(ErrorMessage = "Movie id is required")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Cine id is required")]
    public int? CineId { get; set; }
}