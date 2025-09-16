namespace Wara.Api.Entities;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
