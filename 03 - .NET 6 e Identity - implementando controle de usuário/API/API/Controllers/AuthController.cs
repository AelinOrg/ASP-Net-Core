using API.Data.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<SessionUserDto>> SignUp(SignUpDto dto)
    {
        return Ok(await _authService.SignUp(dto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<SessionUserDto>> Login(LoginDto dto)
    {
        return Ok(await _authService.Login(dto));
    }
}