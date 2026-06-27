using Microsoft.Extensions.DependencyInjection;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Application.Services;
using AirportTicketBooking.Infrastructure.Parsers;
using AirportTicketBooking.Infrastructure.Repositories;
using AirportTicketBooking.UI.Controllers;

namespace AirportTicketBooking.UI;

/// <summary>
/// Centralises all service registrations so Program.cs stays a thin
/// composition root with a single method call.
/// </summary>
internal static class AppConfiguration
{
    /// <summary>
    /// Registers every service the application needs and returns the
    /// populated <see cref="IServiceCollection"/> for fluent chaining.
    /// </summary>
    internal static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string flightsPath,
        string bookingsPath)
    {
        // Infrastructure — repositories receive their file path as a constructor arg.
        services.AddSingleton<IFlightRepository>(_ => new FileFlightRepository(flightsPath));
        services.AddSingleton<IBookingRepository>(_ => new FileBookingRepository(bookingsPath));

        // Infrastructure — CSV parser
        services.AddSingleton<IFlightBatchImporter, CsvFlightImportAdapter>();

        // Application — data seeder (scoped; resolved once during startup)
        services.AddScoped<ISeedData, SeedDataService>();

        // Application — domain services
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IMetadataService, MetadataService>();
        services.AddScoped<IBatchUploadService, BatchUploadService>();

        // UI — controllers
        services.AddScoped<PassengerController>();
        services.AddScoped<ManagerController>();
        services.AddScoped<MainMenuController>();

        return services;
    }
}
