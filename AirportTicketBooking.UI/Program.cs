using Microsoft.Extensions.DependencyInjection;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Application.Services;
using AirportTicketBooking.Infrastructure.Parsers;
using AirportTicketBooking.Infrastructure.Repositories;
using AirportTicketBooking.UI.Controllers;

// ── Data file paths ──────────────────────────────────────────────────────────
// Resolve paths relative to the executable so they work regardless of the
// working directory the user launches from.
var baseDir      = AppContext.BaseDirectory;
var flightsPath  = Path.Combine(baseDir, "data", "flights.json");
var bookingsPath = Path.Combine(baseDir, "data", "bookings.json");

// ── Service registration ─────────────────────────────────────────────────────
var services = new ServiceCollection();

// Infrastructure — repositories receive their file path as a constructor arg.
services.AddSingleton<IFlightRepository>(_ => new FileFlightRepository(flightsPath));
services.AddSingleton<IBookingRepository>(_ => new FileBookingRepository(bookingsPath));

// Infrastructure — CSV parser
services.AddSingleton<IFlightBatchImporter, CsvFlightImportAdapter>();

// Application — services
services.AddScoped<IFlightService, FlightService>();
services.AddScoped<IBookingService, BookingService>();
services.AddScoped<IMetadataService, MetadataService>();
services.AddScoped<IBatchUploadService, BatchUploadService>();

// UI — controllers
services.AddScoped<PassengerController>();
services.AddScoped<ManagerController>();
services.AddScoped<MainMenuController>();

var provider = services.BuildServiceProvider();

// ── Entry point ──────────────────────────────────────────────────────────────
await provider.GetRequiredService<MainMenuController>().StartAsync();
