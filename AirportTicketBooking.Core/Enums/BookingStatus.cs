namespace AirportTicketBooking.Core.Enums;

/// <summary>
/// Represents the lifecycle state of a passenger's flight booking.
/// </summary>
public enum BookingStatus
{
    /// <summary>The booking has been confirmed and is active.</summary>
    Confirmed = 1,

    /// <summary>The booking has been cancelled by the passenger or manager.</summary>
    Cancelled = 2,

    /// <summary>The booking has been modified from its original state.</summary>
    Modified = 3,

    /// <summary>The booking is pending confirmation (e.g., awaiting payment).</summary>
    Pending = 4
}
