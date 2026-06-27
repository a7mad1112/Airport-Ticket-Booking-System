using AirportTicketBooking.Core.Entities;
using AirportTicketBooking.Core.Enums;

namespace AirportTicketBooking.Application.Interfaces;

public interface IBookingService
{
    Task<Booking> BookFlightAsync(Guid passengerId, Guid flightId, FlightClass flightClass);
    Task CancelBookingAsync(Guid bookingId, Guid passengerId);
    Task<Booking> ModifyBookingAsync(Guid bookingId, Guid passengerId, Guid newFlightId, FlightClass newClass);
    Task<IEnumerable<Booking>> GetPassengerBookingsAsync(Guid passengerId);
}
