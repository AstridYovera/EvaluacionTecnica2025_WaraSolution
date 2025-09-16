using Microsoft.EntityFrameworkCore;
using Wara.Api.Data;
using Wara.Api.DTOs;
using Wara.Api.Entities;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;
    public BookingService(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<(bool ok, string? error)> CreateAsync(int userId, int roomId, DateTime date, TimeSpan start, TimeSpan end, string purpose)
    {
        if (string.IsNullOrWhiteSpace(purpose))
            return (false, "El motivo es obligatorio.");

        if (start >= end)
            return (false, "La hora de inicio debe ser menor que la hora de fin.");

        var roomExists = await _db.Rooms.AnyAsync(r => r.Id == roomId);
        if (!roomExists) return (false, "La sala no existe.");

        var startDtLocal = date.Date + start;
        var endDtLocal = date.Date + end;

        var startDt = DateTime.SpecifyKind(startDtLocal, DateTimeKind.Local).ToUniversalTime();
        var endDt = DateTime.SpecifyKind(endDtLocal, DateTimeKind.Local).ToUniversalTime();

        var overlap = await _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            startDt < b.End &&
            b.Start < endDt
        );

        if (startDt <= DateTime.UtcNow)
            return (false, "No se puede reservar en el pasado.");

        if (overlap) return (false, "La sala no está disponible en ese rango.");

        var booking = new Booking
        {
            RoomId = roomId,
            UserId = userId,
            Start = startDt,
            End = endDt,
            Purpose = purpose
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FirstAsync(u => u.Id == userId);
        var room = await _db.Rooms.FirstAsync(r => r.Id == roomId);

        var subject = $"Reserva confirmada - {room.Name}";
        var html = $@"
        <div style='font-family:Segoe UI,Arial,sans-serif; max-width:600px; margin:auto; border:1px solid #e0e0e0; border-radius:8px; padding:20px; background:#f9f9f9;'>
        <h2 style='color:#2a4d9b; text-align:center;'>✅ Reserva Confirmada</h2>
  
          <p style='font-size:16px; color:#333;'>Estimado/a,</p>
          <p style='font-size:15px; color:#555;'>
            Su reserva ha sido registrada correctamente en el sistema <b>WARA</b>.
            A continuación, encontrará los detalles:
          </p>

          <table style='width:100%; border-collapse:collapse; margin:20px 0;'>
            <tr style='background:#2a4d9b; color:#fff; text-align:left;'>
              <th style='padding:10px;'>Detalle</th>
              <th style='padding:10px;'>Información</th>
            </tr>
            <tr style='background:#fff;'>
              <td style='padding:10px; border:1px solid #ddd;'>Sala</td>
              <td style='padding:10px; border:1px solid #ddd;'>{room.Name}</td>
            </tr>
            <tr style='background:#f7f7f7;'>
              <td style='padding:10px; border:1px solid #ddd;'>Fecha</td>
              <td style='padding:10px; border:1px solid #ddd;'>{date:yyyy-MM-dd}</td>
            </tr>
            <tr style='background:#fff;'>
              <td style='padding:10px; border:1px solid #ddd;'>Inicio</td>
              <td style='padding:10px; border:1px solid #ddd;'>{start}</td>
            </tr>
            <tr style='background:#f7f7f7;'>
              <td style='padding:10px; border:1px solid #ddd;'>Fin</td>
              <td style='padding:10px; border:1px solid #ddd;'>{end}</td>
            </tr>
            <tr style='background:#fff;'>
              <td style='padding:10px; border:1px solid #ddd;'>Motivo</td>
              <td style='padding:10px; border:1px solid #ddd;'>{purpose}</td>
            </tr>
          </table>

          <p style='font-size:14px; color:#555;'>
            📅 Recuerde llegar a tiempo para el uso de la sala y notificar en caso de cancelación.
          </p>

          <p style='font-size:13px; color:#777; text-align:center; margin-top:20px;'>
            — Equipo WARA<br/>
            Sistema de Gestión de Reservas
          </p>
        </div>";

        await _email.SendAsync(user.Email, subject, html);

        return (true, null);
    }

    public async Task<IEnumerable<BookingDto>> GetMineAsync(int userId, DateTime? from, DateTime? to)
    {
        var q = _db.Bookings
            .Where(b => b.UserId == userId);

        if (from.HasValue) q = q.Where(b => b.End >= from.Value);
        if (to.HasValue) q = q.Where(b => b.Start <= to.Value);

        return await q
            .OrderBy(b => b.Start)
            .Select(b => new BookingDto(
                b.Id,
                b.RoomId,
                b.Room!.Name,
                b.Start,
                b.End,
                b.Purpose,
                b.UserId,
                b.User!.Username
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(DateTime? from, DateTime? to)
    {
        var q = _db.Bookings.AsQueryable();

        if (from.HasValue) q = q.Where(b => b.End >= from.Value);
        if (to.HasValue) q = q.Where(b => b.Start <= to.Value);

        return await q
            .OrderBy(b => b.Start)
            .Select(b => new BookingDto(
                b.Id,
                b.RoomId,
                b.Room!.Name,
                b.Start,
                b.End,
                b.Purpose,
                b.UserId,
                b.User!.Username
            ))
            .ToListAsync();
    }

}
