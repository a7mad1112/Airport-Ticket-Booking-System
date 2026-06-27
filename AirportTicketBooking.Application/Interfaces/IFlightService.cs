using AirportTicketBooking.Application.DTOs;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<Flight>> SearchAsync(FlightSearchCriteria criteria);
    Task<Flight?> GetByIdAsync(Guid id);
}
