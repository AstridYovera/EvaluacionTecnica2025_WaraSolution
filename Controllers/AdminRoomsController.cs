using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wara.Api.Data;
using Wara.Api.DTOs;
using Wara.Api.Entities;

namespace Wara.Api.Controllers;

[ApiController]
[Route("api/admin/rooms")]
[Authorize(Roles = "Admin")]
public class AdminRoomsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRoomsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoomCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Capacity <= 0)
            return BadRequest(new { error = "Name y Capacity son obligatorios." });

        var exists = await _db.Rooms.AnyAsync(r => r.Name == req.Name);
        if (exists) return BadRequest(new { error = "Ya existe una sala con ese nombre." });

        var room = new Room { Name = req.Name, Capacity = req.Capacity };
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        return room is null ? NotFound() : Ok(room);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateRequest req)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null) return NotFound();

        if (string.IsNullOrWhiteSpace(req.Name) || req.Capacity <= 0)
            return BadRequest(new { error = "Name y Capacity son obligatorios." });

        var dup = await _db.Rooms.AnyAsync(r => r.Name == req.Name && r.Id != id);
        if (dup) return BadRequest(new { error = "Ya existe otra sala con ese nombre." });

        room.Name = req.Name;
        room.Capacity = req.Capacity;
        await _db.SaveChangesAsync();
        return Ok(room);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null) return NotFound();

        var hasFutureBookings = await _db.Bookings.AnyAsync(b => b.RoomId == id && b.Start > DateTime.UtcNow);
        if (hasFutureBookings) return BadRequest(new { error = "No se puede eliminar: tiene reservas futuras." });

        _db.Rooms.Remove(room);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("/api/admin/summary")]
    public async Task<IActionResult> Summary()
    {
        var now = DateTime.UtcNow;

        var totalRooms = await _db.Rooms.CountAsync();

        var occupiedNow = await _db.Bookings
            .Where(b => b.Start <= now && b.End > now)
            .Select(b => b.RoomId)
            .Distinct()
            .CountAsync();

        var availableNow = totalRooms - occupiedNow;

        var next24hBookings = await _db.Bookings
            .Where(b => b.Start > now && b.Start <= now.AddHours(24))
            .CountAsync();

        return Ok(new
        {
            totalRooms,
            occupiedNow,
            availableNow,
            next24hBookings
        });
    }

}
