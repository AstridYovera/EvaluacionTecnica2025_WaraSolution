namespace Wara.Api.DTOs;

public record RoomCreateRequest(string Name, int Capacity);
public record RoomUpdateRequest(string Name, int Capacity);
