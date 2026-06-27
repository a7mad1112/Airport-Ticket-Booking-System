namespace AirportTicketBooking.Application.Interfaces;

public interface ISeedData
{
    /// <summary>
    /// Populates the data store with seed flights (idempotent — runs only when
    /// the store is empty) and writes a sample CSV to the imports directory.
    /// </summary>
    Task InitializeAsync();
}
