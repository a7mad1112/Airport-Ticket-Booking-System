using System.ComponentModel.DataAnnotations;

namespace AirportTicketBooking.Core.Entities;

public sealed class Passenger
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(ErrorMessage = "Passenger name is required.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 150 characters.")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    // 254 characters is the maximum length defined by RFC 5321.
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(254, ErrorMessage = "Email must not exceed 254 characters.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    // Not persisted to JSON; populated in-memory by the application layer.
    public ICollection<Booking> Bookings { get; set; } = [];

    public override string ToString() => $"{Name} ({Email})";
}
