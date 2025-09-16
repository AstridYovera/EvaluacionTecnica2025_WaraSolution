using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wara.Api.DTOs;
using Wara.Api.Services;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login( LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Usuario y Contraseña obligatorio");

        var result = await _authService.LoginAsync(request);

        if (result is null) return Unauthorized(new { error = "Credenciales inválidas" });

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,            
            Secure = true,               
            SameSite = SameSiteMode.Lax, 
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        };
        Response.Cookies.Append("jwt", result.Token, cookieOptions);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (ok, error) = await _authService.RegisterAsync(request);
        return ok ? Ok(new { message = "Usuario registrado correctamente." })
                  : BadRequest(new { error });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var usr = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "";
        var rol = User.FindFirstValue(ClaimTypes.Role) ?? "";
        return Ok(new { id, username = usr, role = rol });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out" });
    }

}
