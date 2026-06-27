using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Application.Services;

/// <summary>
/// Runs once on startup.  Two responsibilities:
///   1. Seed the flights JSON store if it is empty (idempotent).
///   2. Write a sample CSV to imports/ so the manager can test batch upload
///      immediately — includes valid rows and intentionally broken rows so
///      the validation error-reporting path is exercised.
/// </summary>
public sealed class SeedDataService : ISeedData
{
    private readonly IFlightRepository _flightRepository;

    // Resolved at construction time so both responsibilities write to the
    // same base directory the executable lives in.
    private readonly string _importsDir;

    public SeedDataService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
        _importsDir = Path.Combine(AppContext.BaseDirectory, "imports");
    }

    public async Task InitializeAsync()
    {
        await SeedFlightsAsync();
        await WriteSampleCsvAsync();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task SeedFlightsAsync()
    {
        var existing = await _flightRepository.GetAllAsync();

        // Guard: skip seeding if flights are already present so re-runs are safe.
        if (existing.Any())
            return;

        var flights = new List<Flight>
        {
            new()
            {
                FlightNumber      = "EK201",
                DepartureCountry  = "United Arab Emirates",
                DepartureAirport  = "DXB",
                DestinationCountry = "United Kingdom",
                ArrivalAirport    = "LHR",
                DepartureDate     = new DateTime(2026, 8, 15, 8, 30, 0),
                BasePrice         = 420.00m,
                TotalSeats        = 350
            },
            new()
            {
                FlightNumber      = "QR145",
                DepartureCountry  = "Qatar",
                DepartureAirport  = "DOH",
                DestinationCountry = "United States",
                ArrivalAirport    = "JFK",
                DepartureDate     = new DateTime(2026, 9, 1, 14, 0, 0),
                BasePrice         = 780.00m,
                TotalSeats        = 300
            },
            new()
            {
                FlightNumber      = "TK002",
                DepartureCountry  = "Turkey",
                DepartureAirport  = "IST",
                DestinationCountry = "Germany",
                ArrivalAirport    = "FRA",
                DepartureDate     = new DateTime(2026, 7, 20, 6, 45, 0),
                BasePrice         = 210.00m,
                TotalSeats        = 250
            },
            new()
            {
                FlightNumber      = "SQ318",
                DepartureCountry  = "Singapore",
                DepartureAirport  = "SIN",
                DestinationCountry = "Australia",
                ArrivalAirport    = "SYD",
                DepartureDate     = new DateTime(2026, 10, 5, 22, 15, 0),
                BasePrice         = 560.00m,
                TotalSeats        = 471
            },
            new()
            {
                FlightNumber      = "LH400",
                DepartureCountry  = "Germany",
                DepartureAirport  = "MUC",
                DestinationCountry = "Japan",
                ArrivalAirport    = "NRT",
                DepartureDate     = new DateTime(2026, 11, 12, 10, 20, 0),
                BasePrice         = 950.00m,
                TotalSeats        = 400
            }
        };

        await _flightRepository.AddRangeAsync(flights);
    }

    private async Task WriteSampleCsvAsync()
    {
        var csvPath = Path.Combine(_importsDir, "sample_flights.csv");

        // Ensure the imports directory exists before writing.
        Directory.CreateDirectory(_importsDir);

        // Skip if the sample already exists so re-runs do not overwrite edits
        // the manager may have made to the file.
        if (File.Exists(csvPath))
            return;

        // ┌─ Header ─────────────────────────────────────────────────────────────┐
        // Uses the human-readable column names supported by FlightCsvMap so the
        // file looks natural in Excel / LibreOffice Calc.
        // ├─ Valid rows (rows 2–4) ───────────────────────────────────────────────┤
        // ├─ Invalid row 5: past DepartureDate → fails [FutureDate] validation   │
        // └─ Invalid row 6: blank FlightNumber → fails [Required] validation     ┘
        const string csvContent =
            "FlightNumber,Departure Country,Departure Airport,Destination Country,Arrival Airport,Departure Date,Base Price (USD),Total Seats\r\n" +
            "AA101,United States,LAX,France,CDG,2026-09-10 09:00,550.00,280\r\n" +
            "BA249,United Kingdom,LHR,Canada,YYZ,2026-10-18 11:30,490.00,320\r\n" +
            "AF006,France,CDG,Japan,NRT,2026-08-25 23:55,870.00,240\r\n" +
            "EX999,Jordan,AMM,Germany,FRA,2024-03-01 07:00,300.00,180\r\n" +  // ← past date
            ",United States,ORD,Spain,MAD,2026-12-05 16:45,620.00,350\r\n";   // ← missing flight number

        await File.WriteAllTextAsync(csvPath, csvContent);
    }
}
