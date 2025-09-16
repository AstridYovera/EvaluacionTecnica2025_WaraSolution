using Wara.Api.DTOs;

namespace Wara.Api.Services.Interfaces;

public interface IBookingService
{
    Task<(bool ok, string? error)> CreateAsync(int userId, int roomId, DateTime date, TimeSpan start, TimeSpan end, string purpose);
    Task<IEnumerable<BookingDto>> GetMineAsync(int userId, DateTime? from, DateTime? to);
    Task<IEnumerable<BookingDto>> GetAllAsync(DateTime? from, DateTime? to);
}
