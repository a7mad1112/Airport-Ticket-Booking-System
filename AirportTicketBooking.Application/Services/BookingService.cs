using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;
using AirportTicketBooking.Core.Enums;

namespace AirportTicketBooking.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightRepository _flightRepository;

    public BookingService(IBookingRepository bookingRepository, IFlightRepository flightRepository)
    {
        _bookingRepository = bookingRepository;
        _flightRepository = flightRepository;
    }

    public async Task<Booking> BookFlightAsync(Guid passengerId, Guid flightId, FlightClass flightClass)
    {
        var flight = await _flightRepository.GetByIdAsync(flightId)
            ?? throw new KeyNotFoundException($"Flight '{flightId}' does not exist.");

        var booking = new Booking
        {
            FlightId    = flightId,
            PassengerId = passengerId,
            Class       = flightClass,
            // Price is locked at booking time using the domain multiplier on Flight
            // so future base-price edits do not retroactively change what was charged.
            PricePaid   = flight.GetPriceForClass(flightClass),
            Status      = BookingStatus.Confirmed,
        };

        await _bookingRepository.AddAsync(booking);
        return booking;
    }

    public async Task CancelBookingAsync(Guid bookingId, Guid passengerId)
    {
        var booking = await GetOwnedBookingAsync(bookingId, passengerId);

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        booking.Status            = BookingStatus.Cancelled;
        booking.LastModifiedAtUtc = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<Booking> ModifyBookingAsync(
        Guid bookingId, Guid passengerId, Guid newFlightId, FlightClass newClass)
    {
        var booking = await GetOwnedBookingAsync(bookingId, passengerId);

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("A cancelled booking cannot be modified.");

        var newFlight = await _flightRepository.GetByIdAsync(newFlightId)
            ?? throw new KeyNotFoundException($"Flight '{newFlightId}' does not exist.");

        booking.FlightId          = newFlightId;
        booking.Class             = newClass;
        booking.PricePaid         = newFlight.GetPriceForClass(newClass);
        booking.Status            = BookingStatus.Modified;
        booking.LastModifiedAtUtc = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);
        return booking;
    }

    public async Task<IEnumerable<Booking>> GetPassengerBookingsAsync(Guid passengerId) =>
        await _bookingRepository.GetByPassengerIdAsync(passengerId);

    // Centralises ownership verification so CancelAsync and ModifyAsync
    // both guard against a passenger tampering with another passenger's booking.
    private async Task<Booking> GetOwnedBookingAsync(Guid bookingId, Guid passengerId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException($"Booking '{bookingId}' does not exist.");

        if (booking.PassengerId != passengerId)
            throw new UnauthorizedAccessException("You do not own this booking.");

        return booking;
    }
}
