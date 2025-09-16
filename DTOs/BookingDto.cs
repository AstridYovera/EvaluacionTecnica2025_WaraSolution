namespace Wara.Api.DTOs;

public record BookingDto(
    int Id,
    int RoomId,
    string RoomName,
    DateTime Start,
    DateTime End,
    string Purpose,
    int UserId,
    string Username
);
