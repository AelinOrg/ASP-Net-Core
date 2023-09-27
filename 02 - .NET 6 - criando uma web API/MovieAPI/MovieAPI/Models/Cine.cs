using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Cine
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public int AddressId { get; set; }
    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = null!;
}
