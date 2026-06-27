using System.ComponentModel.DataAnnotations;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Services;

public sealed class BatchUploadService : IBatchUploadService
{
    private readonly IFlightBatchImporter _importer;
    private readonly IFlightRepository _flightRepository;

    public BatchUploadService(IFlightBatchImporter importer, IFlightRepository flightRepository)
    {
        _importer = importer;
        _flightRepository = flightRepository;
    }

    public async Task<Dictionary<int, List<string>>> ImportFlightsAsync(string filePath)
    {
        var flights = _importer.Import(filePath).ToList();
        var errors  = ValidateAll(flights);

        // All-or-nothing: if any row is invalid, reject the entire batch.
        // This prevents partial imports that leave the data in an inconsistent state.
        if (errors.Count > 0)
            return errors;

        await _flightRepository.AddRangeAsync(flights);
        return [];
    }

    private static Dictionary<int, List<string>> ValidateAll(List<Flight> flights)
    {
        var errors = new Dictionary<int, List<string>>();

        for (var i = 0; i < flights.Count; i++)
        {
            var rowNumber       = i + 2; // +2 because row 1 is the CSV header
            var validationResults = new List<ValidationResult>();
            var context         = new ValidationContext(flights[i]);

            // validateAllProperties: true — runs every attribute, not just [Required].
            var isValid = Validator.TryValidateObject(
                flights[i], context, validationResults, validateAllProperties: true);

            if (!isValid)
                errors[rowNumber] = validationResults
                    .Select(r => r.ErrorMessage ?? "Unknown validation error.")
                    .ToList();
        }

        return errors;
    }
}
