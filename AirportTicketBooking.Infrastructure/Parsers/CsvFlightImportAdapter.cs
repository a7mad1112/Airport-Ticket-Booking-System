using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Infrastructure.Parsers;

public sealed class CsvFlightImportAdapter : IFlightBatchImporter
{
    private static readonly CsvConfiguration CsvConfig = new(CultureInfo.InvariantCulture)
    {
        // Trim whitespace from each field value so "  Jordan  " == "Jordan".
        TrimOptions        = TrimOptions.Trim,
        // Skip blank lines silently rather than throwing on them.
        ShouldSkipRecord   = args => args.Row.Parser.Record?.All(string.IsNullOrWhiteSpace) ?? false,
        // Case-insensitive header matching handles "flightnumber" vs "FlightNumber".
        PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
        MissingFieldFound  = null, // Suppress exceptions for optional/unknown columns.
    };

    public IEnumerable<Flight> Import(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        using var reader = new StreamReader(filePath);
        using var csv    = new CsvReader(reader, CsvConfig);

        csv.Context.RegisterClassMap<FlightCsvMap>();

        // GetRecords uses deferred streaming; ToList() forces eager evaluation
        // here so the StreamReader can be disposed safely before we return.
        return csv.GetRecords<Flight>().ToList();
    }
}
