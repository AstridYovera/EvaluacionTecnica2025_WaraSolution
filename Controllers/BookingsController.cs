using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wara.Api.Data;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _svc;
    public BookingsController(IBookingService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DTOs.BookingRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (ok, error) = await _svc.CreateAsync(userId, req.RoomId, req.Date, req.StartTime, req.EndTime, req.Purpose);
        return ok ? Ok(new { message = "Reserva creada correctamente." }) : BadRequest(new { error });
    }

    [HttpGet("my")]
    public async Task<IActionResult> My([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var data = await _svc.GetMineAsync(userId, from, to);
        return Ok(data);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> All([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var data = await _svc.GetAllAsync(from, to);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id, [FromServices] AppDbContext db)
    {
        var booking = await db.Bookings
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!isAdmin && booking.UserId != userId)
            return Forbid();

        if (booking.Start <= DateTime.UtcNow)
            return BadRequest(new { error = "No se puede cancelar una reserva ya iniciada o pasada." });

        db.Bookings.Remove(booking);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
