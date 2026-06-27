using AirportTicketBooking.Application.DTOs;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;
using AirportTicketBooking.Core.Enums;
using AirportTicketBooking.UI.Views;

namespace AirportTicketBooking.UI.Controllers;

public sealed class PassengerController
{
    private readonly IFlightService _flightService;
    private readonly IBookingService _bookingService;

    // In a real system the passenger would log in; here we generate a stable ID
    // for the session so bookings can be scoped to this "user".
    private readonly Guid _passengerId = Guid.NewGuid();

    public PassengerController(IFlightService flightService, IBookingService bookingService)
    {
        _flightService  = flightService;
        _bookingService = bookingService;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            PrintMenu();
            var choice = ConsoleHelper.PromptMenuChoice(4);

            switch (choice)
            {
                case 1: await SearchFlightsAsync();    break;
                case 2: await BookFlightAsync();       break;
                case 3: await ViewMyBookingsAsync();   break;
                case 4: await CancelBookingAsync();    break;
                case 5: await ModifyBookingAsync();    break;
                case 0: return;
            }
        }
    }

    private static void PrintMenu()
    {
        ConsoleHelper.PrintSectionHeader("Passenger Menu");
        Console.WriteLine("  1. Search available flights");
        Console.WriteLine("  2. Book a flight");
        Console.WriteLine("  3. View my bookings");
        Console.WriteLine("  4. Cancel a booking");
        Console.WriteLine("  5. Modify a booking");
        Console.WriteLine("  0. Back to main menu");
        ConsoleHelper.PrintSectionFooter();
    }

    private async Task SearchFlightsAsync()
    {
        ConsoleHelper.PrintSectionHeader("Search Flights");

        var maxPrice          = ConsoleHelper.PromptDecimal("Max price (USD)");
        var departureCountry  = ConsoleHelper.PromptOptional("Departure country");
        var destinationCountry = ConsoleHelper.PromptOptional("Destination country");
        var departureDate     = ConsoleHelper.PromptDate("Departure date");
        var departureAirport  = ConsoleHelper.PromptOptional("Departure airport (IATA)");
        var arrivalAirport    = ConsoleHelper.PromptOptional("Arrival airport (IATA)");

        var criteria = new FlightSearchCriteria(
            MaxPrice:           maxPrice,
            DepartureCountry:   departureCountry,
            DestinationCountry: destinationCountry,
            DepartureDate:      departureDate,
            DepartureAirport:   departureAirport,
            ArrivalAirport:     arrivalAirport
        );

        var flights = await _flightService.SearchAsync(criteria);
        ConsoleHelper.PrintFlightsTable(flights);
    }

    private async Task BookFlightAsync()
    {
        ConsoleHelper.PrintSectionHeader("Book a Flight");

        var flightId = ConsoleHelper.PromptGuid("Flight ID");
        if (flightId is null) return;

        var flight = await _flightService.GetByIdAsync(flightId.Value);
        if (flight is null)
        {
            ConsoleHelper.PrintError("Flight not found.");
            return;
        }

        Console.WriteLine($"\n  Flight: {flight}");
        var flightClass = PromptFlightClass();
        if (flightClass is null) return;

        var finalPrice = flight.GetPriceForClass(flightClass.Value);
        Console.WriteLine($"\n  Class: {flightClass}  |  Price: ${finalPrice:F2}");

        if (!ConsoleHelper.PromptYesNo("Confirm booking?")) return;

        try
        {
            var booking = await _bookingService.BookFlightAsync(_passengerId, flightId.Value, flightClass.Value);
            ConsoleHelper.PrintSuccess($"Booking confirmed! ID: {booking.Id}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ViewMyBookingsAsync()
    {
        var bookings = await _bookingService.GetPassengerBookingsAsync(_passengerId);
        ConsoleHelper.PrintBookingsTable(bookings);
    }

    private async Task CancelBookingAsync()
    {
        ConsoleHelper.PrintSectionHeader("Cancel Booking");

        var bookingId = ConsoleHelper.PromptGuid("Booking ID to cancel");
        if (bookingId is null) return;

        if (!ConsoleHelper.PromptYesNo("Are you sure you want to cancel?")) return;

        try
        {
            await _bookingService.CancelBookingAsync(bookingId.Value, _passengerId);
            ConsoleHelper.PrintSuccess("Booking cancelled.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ModifyBookingAsync()
    {
        ConsoleHelper.PrintSectionHeader("Modify Booking");

        var bookingId = ConsoleHelper.PromptGuid("Booking ID to modify");
        if (bookingId is null) return;

        var newFlightId = ConsoleHelper.PromptGuid("New Flight ID");
        if (newFlightId is null) return;

        var newFlight = await _flightService.GetByIdAsync(newFlightId.Value);
        if (newFlight is null)
        {
            ConsoleHelper.PrintError("New flight not found.");
            return;
        }

        var newClass = PromptFlightClass();
        if (newClass is null) return;

        var newPrice = newFlight.GetPriceForClass(newClass.Value);
        Console.WriteLine($"\n  New class: {newClass}  |  New price: ${newPrice:F2}");

        if (!ConsoleHelper.PromptYesNo("Confirm modification?")) return;

        try
        {
            await _bookingService.ModifyBookingAsync(bookingId.Value, _passengerId, newFlightId.Value, newClass.Value);
            ConsoleHelper.PrintSuccess("Booking modified successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private static FlightClass? PromptFlightClass()
    {
        Console.WriteLine("\n  Select class:");
        Console.WriteLine("    1. Economy   (×1.0)");
        Console.WriteLine("    2. Business  (×1.5)");
        Console.WriteLine("    3. First Class (×2.5)");
        Console.Write("  Choice: ");

        return Console.ReadLine()?.Trim() switch
        {
            "1" => FlightClass.Economy,
            "2" => FlightClass.Business,
            "3" => FlightClass.FirstClass,
            _   => null
        };
    }
}
