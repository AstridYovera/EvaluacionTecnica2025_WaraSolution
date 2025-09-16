using Wara.Api.DTOs;

namespace Wara.Api.Services.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAsync(int? capacityMin, string? status, DateTime? whenUtc);
}
