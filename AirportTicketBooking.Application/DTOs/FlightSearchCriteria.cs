using AirportTicketBooking.Core.Enums;

namespace AirportTicketBooking.Application.DTOs;

public sealed record FlightSearchCriteria(
    decimal? MaxPrice = null,
    string? DepartureCountry = null,
    string? DestinationCountry = null,
    DateTime? DepartureDate = null,
    string? DepartureAirport = null,
    string? ArrivalAirport = null,
    FlightClass? Class = null
);
