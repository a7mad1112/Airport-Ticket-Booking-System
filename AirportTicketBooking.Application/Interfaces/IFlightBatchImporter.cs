using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Interfaces;

public interface IFlightBatchImporter
{
    // Parses the CSV at filePath and returns the raw, unvalidated Flight objects.
    // Validation is deliberately kept out of this interface so the Application
    // layer (BatchUploadService) owns the validation pipeline and can report
    // errors without coupling the importer to DataAnnotations.
    IEnumerable<Flight> Import(string filePath);
}
