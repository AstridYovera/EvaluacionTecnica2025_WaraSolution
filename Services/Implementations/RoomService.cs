using Microsoft.EntityFrameworkCore;
using Wara.Api.Data;
using Wara.Api.DTOs;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly AppDbContext _db;
    public RoomService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<RoomDto>> GetAsync(int? capacityMin, string? status, DateTime? whenUtc)
    {
        var when = (whenUtc ?? DateTime.UtcNow).ToUniversalTime();

        var query = _db.Rooms
            .AsNoTracking() 
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Capacity,
                IsOccupied = _db.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.Start <= when &&
                    when < b.End)
            });

        if (capacityMin.HasValue)
            query = query.Where(x => x.Capacity >= capacityMin.Value);

        var items = await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim();
            bool? wantOccupied =
                s.Equals("Ocupada", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("Ocupado", StringComparison.OrdinalIgnoreCase) ? true :
                s.Equals("Disponible", StringComparison.OrdinalIgnoreCase) ? false :
                null;

            if (wantOccupied.HasValue)
                items = items.Where(x => x.IsOccupied == wantOccupied.Value).ToList();
            else
                items.Clear();
        }

        return items.Select(x => new RoomDto(
            x.Id, x.Name, x.Capacity, x.IsOccupied ? "Ocupada" : "Disponible"));
    }

}
