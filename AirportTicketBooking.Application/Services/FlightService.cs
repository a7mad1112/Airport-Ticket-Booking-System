using AirportTicketBooking.Application.DTOs;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Services;

public sealed class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    public FlightService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<IEnumerable<Flight>> SearchAsync(FlightSearchCriteria criteria)
    {
        var flights = await _flightRepository.GetAllAsync();

        // Each predicate is only applied when the caller supplied a non-null value,
        // so an empty criteria object returns the full unfiltered list.
        return flights
            .Where(f => criteria.MaxPrice is null || f.BasePrice <= criteria.MaxPrice)
            .Where(f => criteria.DepartureCountry is null ||
                        f.DepartureCountry.Equals(criteria.DepartureCountry, StringComparison.OrdinalIgnoreCase))
            .Where(f => criteria.DestinationCountry is null ||
                        f.DestinationCountry.Equals(criteria.DestinationCountry, StringComparison.OrdinalIgnoreCase))
            .Where(f => criteria.DepartureDate is null ||
                        f.DepartureDate.Date == criteria.DepartureDate.Value.Date)
            .Where(f => criteria.DepartureAirport is null ||
                        f.DepartureAirport.Equals(criteria.DepartureAirport, StringComparison.OrdinalIgnoreCase))
            .Where(f => criteria.ArrivalAirport is null ||
                        f.ArrivalAirport.Equals(criteria.ArrivalAirport, StringComparison.OrdinalIgnoreCase));

        // Class is a search hint for the passenger (they want to know if a class is
        // available at a certain price), but all flights support all three classes.
        // Price filtering by class is deferred to the booking step via GetPriceForClass.
    }

    public async Task<Flight?> GetByIdAsync(Guid id) =>
        await _flightRepository.GetByIdAsync(id);
}
