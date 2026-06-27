using System.ComponentModel.DataAnnotations;
using AirportTicketBooking.Core.Validation;

namespace AirportTicketBooking.Core.Entities;

public sealed class Flight
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(ErrorMessage = "Flight number is required.")]
    [StringLength(10, MinimumLength = 2,
        ErrorMessage = "Flight number must be between 2 and 10 characters.")]
    [Display(Name = "Flight Number")]
    public string FlightNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Departure country is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Departure country must be between 2 and 100 characters.")]
    [Display(Name = "Departure Country")]
    public string DepartureCountry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Departure airport is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Departure airport must be between 3 and 100 characters.")]
    [Display(Name = "Departure Airport")]
    public string DepartureAirport { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination country is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Destination country must be between 2 and 100 characters.")]
    [Display(Name = "Destination Country")]
    public string DestinationCountry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Arrival airport is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Arrival airport must be between 3 and 100 characters.")]
    [Display(Name = "Arrival Airport")]
    public string ArrivalAirport { get; set; } = string.Empty;

    [Required(ErrorMessage = "Departure date is required.")]
    [FutureDate]
    [Display(Name = "Departure Date",
        Description = "Required, Allowed Range (today → future)")]
    public DateTime DepartureDate { get; set; }

    // BasePrice is the Economy baseline. Final price = BasePrice × class multiplier:
    // Economy × 1.0 | Business × 1.5 | First Class × 2.5
    [Required(ErrorMessage = "Base price is required.")]
    [Range(1.00, 100_000.00,
        ErrorMessage = "Base price must be between $1.00 and $100,000.00.")]
    [Display(Name = "Base Price (USD)",
        Description = "Economy base price; multiply by class factor for final price.")]
    [DataType(DataType.Currency)]
    public decimal BasePrice { get; set; }

    // Upper bound of 853 matches the certified maximum capacity of an Airbus A380.
    [Required(ErrorMessage = "Total seats is required.")]
    [Range(1, 853,
        ErrorMessage = "Total seats must be between 1 and 853 (max Airbus A380 capacity).")]
    [Display(Name = "Total Seats")]
    public int TotalSeats { get; set; }

    public decimal GetPriceForClass(Enums.FlightClass flightClass) =>
        flightClass switch
        {
            Enums.FlightClass.Economy    => BasePrice * 1.0m,
            Enums.FlightClass.Business   => BasePrice * 1.5m,
            Enums.FlightClass.FirstClass => BasePrice * 2.5m,
            _ => throw new ArgumentOutOfRangeException(nameof(flightClass), flightClass, null)
        };

    public override string ToString() =>
        $"[{FlightNumber}] {DepartureAirport} → {ArrivalAirport} on {DepartureDate:yyyy-MM-dd HH:mm} | From ${BasePrice:F2}";
}
