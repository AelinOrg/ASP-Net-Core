using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User: IdentityUser
{
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
}
