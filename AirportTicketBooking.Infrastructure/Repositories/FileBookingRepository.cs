using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Infrastructure.Repositories;

public sealed class FileBookingRepository : JsonFileRepository<Booking>, IBookingRepository
{
    public FileBookingRepository(string filePath) : base(filePath) { }

    public async Task<IEnumerable<Booking>> GetAllAsync() =>
        await ReadAllAsync();

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        var bookings = await ReadAllAsync();
        return bookings.FirstOrDefault(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetByPassengerIdAsync(Guid passengerId)
    {
        var bookings = await ReadAllAsync();
        return bookings.Where(b => b.PassengerId == passengerId);
    }

    public async Task AddAsync(Booking booking)
    {
        var bookings = await ReadAllAsync();
        bookings.Add(booking);
        await WriteAllAsync(bookings);
    }

    public async Task UpdateAsync(Booking booking)
    {
        var bookings = await ReadAllAsync();
        var index = bookings.FindIndex(b => b.Id == booking.Id);

        if (index == -1)
            throw new KeyNotFoundException($"Booking with ID '{booking.Id}' was not found.");

        bookings[index] = booking;
        await WriteAllAsync(bookings);
    }

    public async Task DeleteAsync(Guid id)
    {
        var bookings = await ReadAllAsync();
        var removed = bookings.RemoveAll(b => b.Id == id);

        if (removed == 0)
            throw new KeyNotFoundException($"Booking with ID '{id}' was not found.");

        await WriteAllAsync(bookings);
    }
}
