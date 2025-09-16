namespace Wara.Api.Entities;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Purpose { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
