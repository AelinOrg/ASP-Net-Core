using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Session
{
    [Required]
    public int? MovieId { get; set; }
    public virtual Movie Movie { get; set; } = null!;

    [Required]
    public int CineId { get; set; }
    public virtual Cine Cine { get; set; }
}

