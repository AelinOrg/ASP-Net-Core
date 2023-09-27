using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User: IdentityUser
{
    public DateTime BirthDate { get; set; }
}
