using Microsoft.Extensions.DependencyInjection;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.UI;
using AirportTicketBooking.UI.Controllers;

// ── Data file paths ──────────────────────────────────────────────────────────
// Resolve paths relative to the executable so they work regardless of the
// working directory the user launches from.
var baseDir      = AppContext.BaseDirectory;
var flightsPath  = Path.Combine(baseDir, "data", "flights.json");
var bookingsPath = Path.Combine(baseDir, "data", "bookings.json");

// ── Service registration ─────────────────────────────────────────────────────
var provider = new ServiceCollection()
    .AddApplicationServices(flightsPath, bookingsPath)
    .BuildServiceProvider();

// ── Data seeding ─────────────────────────────────────────────────────────────
// Runs before the UI starts. Both operations are idempotent:
//   • Flights are seeded only when the store is empty.
//   • The sample CSV is written only if it does not already exist.
await provider.GetRequiredService<ISeedData>().InitializeAsync();

// ── Entry point ──────────────────────────────────────────────────────────────
await provider.GetRequiredService<MainMenuController>().StartAsync();
