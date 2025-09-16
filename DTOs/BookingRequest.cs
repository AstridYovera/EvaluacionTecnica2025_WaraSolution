namespace Wara.Api.DTOs;

public class BookingRequest
{
    public int RoomId { get; set; }
    public DateTime Date { get; set; } 
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Purpose { get; set; } = null!;
}
