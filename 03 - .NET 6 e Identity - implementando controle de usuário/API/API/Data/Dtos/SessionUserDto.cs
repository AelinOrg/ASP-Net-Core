using API.Models;

namespace API.Data.Dtos;

public class SessionUserDto
{


    public User User { get; set; } = null!;
    public string Token { get; set; } = null!;

}
