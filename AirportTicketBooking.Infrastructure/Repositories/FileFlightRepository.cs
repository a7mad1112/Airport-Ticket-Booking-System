using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Infrastructure.Repositories;

public sealed class FileFlightRepository : JsonFileRepository<Flight>, IFlightRepository
{
    public FileFlightRepository(string filePath) : base(filePath) { }

    public async Task<IEnumerable<Flight>> GetAllAsync() =>
        await ReadAllAsync();

    public async Task<Flight?> GetByIdAsync(Guid id)
    {
        var flights = await ReadAllAsync();
        return flights.FirstOrDefault(f => f.Id == id);
    }

    public async Task AddAsync(Flight flight)
    {
        var flights = await ReadAllAsync();
        flights.Add(flight);
        await WriteAllAsync(flights);
    }

    public async Task AddRangeAsync(IEnumerable<Flight> flights)
    {
        var existing = await ReadAllAsync();
        existing.AddRange(flights);
        await WriteAllAsync(existing);
    }

    public async Task UpdateAsync(Flight flight)
    {
        var flights = await ReadAllAsync();
        var index = flights.FindIndex(f => f.Id == flight.Id);

        if (index == -1)
            throw new KeyNotFoundException($"Flight with ID '{flight.Id}' was not found.");

        flights[index] = flight;
        await WriteAllAsync(flights);
    }

    public async Task DeleteAsync(Guid id)
    {
        var flights = await ReadAllAsync();
        var removed = flights.RemoveAll(f => f.Id == id);

        if (removed == 0)
            throw new KeyNotFoundException($"Flight with ID '{id}' was not found.");

        await WriteAllAsync(flights);
    }
}
