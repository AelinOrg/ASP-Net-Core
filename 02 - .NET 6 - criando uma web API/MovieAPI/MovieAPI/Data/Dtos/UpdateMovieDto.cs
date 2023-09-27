using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class UpdateMovieDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    [StringLength(20, ErrorMessage = "Genre cannot be longer than 20 characters")]
    public string Genre { get; set; }

    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
    public int Duration { get; set; }
}