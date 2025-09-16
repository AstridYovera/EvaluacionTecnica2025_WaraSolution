using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wara.Api.Services.Implementations;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Controllers;

[ApiController]
[Route("api/rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _svc;
    public RoomsController(IRoomService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] int? capacityMin,
    [FromQuery] string? status,
    [FromQuery] DateTime? at)
    {
        var when = at.HasValue
            ? DateTime.SpecifyKind(at.Value, DateTimeKind.Utc)
        : DateTime.UtcNow;

        var data = await _svc.GetAsync(capacityMin, status, when);

        if (!data.Any())
            return Ok(new { message = "No hay resultados", data });

        return Ok(data);
    }
}
