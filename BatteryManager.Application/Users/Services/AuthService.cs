using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BatteryManager.Application.Users.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using BatteryManager.Domain.Entities;

namespace BatteryManager.Application.Users.Services;

public class AuthService
{
    private readonly IUsersRepository _userRepo;
    private readonly IConfiguration _config;


    public async Task CreateUserAsync(User user)
    {
        await _userRepo.CreateAsync(user);
    }

    public AuthService(IUsersRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<string?> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password)
    {
        byte[] salt = Encoding.UTF8.GetBytes("your-static-salt");
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));
        return hashed;
    }

    public bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}