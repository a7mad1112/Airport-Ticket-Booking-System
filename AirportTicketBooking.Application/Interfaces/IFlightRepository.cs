using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Interfaces;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> GetAllAsync();
    Task<Flight?> GetByIdAsync(Guid id);
    Task AddAsync(Flight flight);
    Task AddRangeAsync(IEnumerable<Flight> flights);
    Task UpdateAsync(Flight flight);
    Task DeleteAsync(Guid id);
}
