using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Attributes;
using API.Data.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

[Service]
public class AuthService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;


    public AuthService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<SessionUserDto> SignUp(SignUpDto dto)
    {
        User user = _mapper.Map<User>(dto);
        IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                throw new BadHttpRequestException(error.Code);
            }
        }

        await _userManager.AddToRoleAsync(user, dto.Role);

        string token = GenerateToken(user);

        return new SessionUserDto { User = user, Token = token };
    }

    public async Task<SessionUserDto> Login(LoginDto dto)
    {
        User? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("EMAIL_OR_PASSWORD_NOT_FOUND");
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("EMAIL_OR_PASSWORD_NOT_FOUND");
        }

        string token = GenerateToken(user);

        return new SessionUserDto { User = user, Token = token };
    }

    private string GenerateToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
