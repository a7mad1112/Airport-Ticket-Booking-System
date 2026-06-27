using AirportTicketBooking.Core.Enums;

namespace AirportTicketBooking.Core.Entities;

public sealed class Booking
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid FlightId { get; set; }
    public Guid PassengerId { get; set; }

    // Not persisted to JSON; resolved in-memory by the application layer.
    public Flight? Flight { get; set; }
    public Passenger? Passenger { get; set; }

    public FlightClass Class { get; set; }

    // Locked-in at booking time so future changes to Flight.BasePrice
    // do not retroactively affect existing reservations.
    public decimal PricePaid { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    public DateTime BookedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? LastModifiedAtUtc { get; set; }

    public override string ToString() =>
        $"Booking [{Id}] | Flight: {FlightId} | Class: {Class} | Price: ${PricePaid:F2} | Status: {Status}";
}
