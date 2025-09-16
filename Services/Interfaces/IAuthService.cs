using Wara.Api.DTOs;

namespace Wara.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<(bool ok, string? error)> RegisterAsync(RegisterRequest request);
}
