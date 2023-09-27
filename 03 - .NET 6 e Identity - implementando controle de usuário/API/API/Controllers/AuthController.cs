using API.Data.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManager<User> _userManager;

    public AuthController(AuthService authService, UserManager<User> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<User>> SignUp(SignUpDto dto)
    {
        return Ok(await _authService.SignUp(dto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        return Ok(await _authService.Login(dto));
    }

    [HttpGet("me")]
    [Authorize("RequireAuth")]
    public async Task<ActionResult<User>> MeAsync()
    {
        return Ok(await _userManager.GetUserAsync(User));
    }
}
