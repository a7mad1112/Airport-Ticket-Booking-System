namespace AirportTicketBooking.Application.Interfaces;

public interface IBatchUploadService
{
    // Returns an empty dictionary on full success.
    // Returns a non-empty dictionary on validation failure — nothing is saved.
    // Key   = 1-based CSV row number (matching the row the manager sees in their spreadsheet).
    // Value = list of validation error messages for that row.
    Task<Dictionary<int, List<string>>> ImportFlightsAsync(string filePath);
}
