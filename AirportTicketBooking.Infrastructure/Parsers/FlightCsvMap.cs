using CsvHelper.Configuration;
using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.Infrastructure.Parsers;

// Explicit ClassMap lets us decouple CSV column names from property names.
// The manager's spreadsheet may use "Departure Country" with a space, while
// the C# property is DepartureCountry — the map bridges that gap without
// requiring the entity to carry CsvHelper attributes.
internal sealed class FlightCsvMap : ClassMap<Flight>
{
    public FlightCsvMap()
    {
        Map(f => f.FlightNumber).Name("FlightNumber", "Flight Number");
        Map(f => f.DepartureCountry).Name("DepartureCountry", "Departure Country");
        Map(f => f.DepartureAirport).Name("DepartureAirport", "Departure Airport");
        Map(f => f.DestinationCountry).Name("DestinationCountry", "Destination Country");
        Map(f => f.ArrivalAirport).Name("ArrivalAirport", "Arrival Airport");
        Map(f => f.DepartureDate).Name("DepartureDate", "Departure Date");
        Map(f => f.BasePrice).Name("BasePrice", "Base Price", "Base Price (USD)");
        Map(f => f.TotalSeats).Name("TotalSeats", "Total Seats");
    }
}
