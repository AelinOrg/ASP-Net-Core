using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Movie
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    [MaxLength(20)]
    public string Genre { get; set; }

    [Required]
    public int Duration { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = null!;
}