namespace AirportTicketBooking.Core.Enums;

/// <summary>
/// Represents the travel class for a flight booking.
/// Each class applies a different pricing multiplier to the base flight price.
/// </summary>
public enum FlightClass
{
    /// <summary>Economy class — multiplier: 1.0x base price.</summary>
    Economy = 1,

    /// <summary>Business class — multiplier: 1.5x base price.</summary>
    Business = 2,

    /// <summary>First class — multiplier: 2.5x base price.</summary>
    FirstClass = 3
}
