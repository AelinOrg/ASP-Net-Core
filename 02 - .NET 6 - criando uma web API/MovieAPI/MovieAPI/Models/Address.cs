using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MovieAPI.Models;

public class Address
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Street { get; set; } = null!;

    [Required]
    public string Number { get; set; } = null!;

    [JsonIgnore]
    public virtual Cine Cine { get; set; } = null!;

}

