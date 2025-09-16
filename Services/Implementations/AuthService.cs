using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wara.Api.DTOs;
using Wara.Api.Data;
using Microsoft.EntityFrameworkCore;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<(bool ok, string? error)> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return (false, "El usuario y la contraseña son obligatorios.");

        var exists = await _db.Users.AnyAsync(u => u.Username == request.Username);
        if (exists) return (false, "El nombre de usuario ya existe.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        _db.Users.Add(new Entities.User
        {
            Username = request.Username,
            PasswordHash = hash,
            Email = request.Email,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
        });

        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(tokenString, user.Role, user.Username);
    }
}
