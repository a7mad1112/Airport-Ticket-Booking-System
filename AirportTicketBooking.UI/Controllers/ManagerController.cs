using AirportTicketBooking.Application.DTOs;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.UI.Views;

namespace AirportTicketBooking.UI.Controllers;

public sealed class ManagerController
{
    private readonly IFlightService _flightService;
    private readonly IBookingService _bookingService;
    private readonly IMetadataService _metadataService;
    private readonly IBatchUploadService _batchUploadService;

    public ManagerController(
        IFlightService flightService,
        IBookingService bookingService,
        IMetadataService metadataService,
        IBatchUploadService batchUploadService)
    {
        _flightService      = flightService;
        _bookingService     = bookingService;
        _metadataService    = metadataService;
        _batchUploadService = batchUploadService;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            PrintMenu();
            var choice = ConsoleHelper.PromptMenuChoice(3);

            switch (choice)
            {
                case 1: await FilterBookingsAsync();     break;
                case 2: ViewValidationConstraints();     break;
                case 3: await BatchImportFlightsAsync(); break;
                case 0: return;
            }
        }
    }

    private static void PrintMenu()
    {
        ConsoleHelper.PrintSectionHeader("Manager Menu");
        Console.WriteLine("  1. Filter bookings");
        Console.WriteLine("  2. View Flight model validation rules");
        Console.WriteLine("  3. Batch import flights from CSV");
        Console.WriteLine("  0. Back to main menu");
        ConsoleHelper.PrintSectionFooter();
    }

    private async Task FilterBookingsAsync()
    {
        ConsoleHelper.PrintSectionHeader("Filter Bookings");
        ConsoleHelper.PrintInfo("Leave any field blank to skip that filter.");

        var maxPrice           = ConsoleHelper.PromptDecimal("Max price paid (USD)");
        var departureCountry   = ConsoleHelper.PromptOptional("Departure country");
        var destinationCountry = ConsoleHelper.PromptOptional("Destination country");
        var departureDate      = ConsoleHelper.PromptDate("Departure date");
        var departureAirport   = ConsoleHelper.PromptOptional("Departure airport");
        var arrivalAirport     = ConsoleHelper.PromptOptional("Arrival airport");

        // First resolve which flight IDs match the route/schedule criteria.
        var matchingFlightIds = (await _flightService.SearchAsync(new FlightSearchCriteria(
            MaxPrice:           null,   // price filter is applied to PricePaid on the booking
            DepartureCountry:   departureCountry,
            DestinationCountry: destinationCountry,
            DepartureDate:      departureDate,
            DepartureAirport:   departureAirport,
            ArrivalAirport:     arrivalAirport
        ))).Select(f => f.Id).ToHashSet();

        // Then filter all bookings against those flight IDs and the price ceiling.
        var bookings = (await _bookingService.GetAllBookingsAsync())
            .Where(b => matchingFlightIds.Count == 0 || matchingFlightIds.Contains(b.FlightId))
            .Where(b => maxPrice is null || b.PricePaid <= maxPrice)
            .ToList();

        ConsoleHelper.PrintBookingsTable(bookings);
    }

    private void ViewValidationConstraints()
    {
        var rules = _metadataService.GetFlightValidationRules();
        ConsoleHelper.PrintValidationRules(rules);
    }

    private async Task BatchImportFlightsAsync()
    {
        ConsoleHelper.PrintSectionHeader("Batch Import Flights");

        var filePath = ConsoleHelper.PromptRequired("Path to CSV file");

        if (!File.Exists(filePath))
        {
            ConsoleHelper.PrintError($"File not found: {filePath}");
            return;
        }

        try
        {
            var errors = await _batchUploadService.ImportFlightsAsync(filePath);

            if (errors.Count == 0)
                ConsoleHelper.PrintSuccess("All flights imported successfully.");
            else
                ConsoleHelper.PrintImportErrors(errors);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Import failed: {ex.Message}");
        }
    }
}
