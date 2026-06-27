using System.ComponentModel.DataAnnotations;

namespace AirportTicketBooking.Core.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
        : base("The {0} must be a future date (today → future).")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            // Null is not our concern here — [Required] handles that separately.
            return ValidationResult.Success;
        }

        var date = (DateTime)value;

        if (date.ToUniversalTime() > DateTime.UtcNow)
            return ValidationResult.Success;

        var memberName = validationContext.MemberName ?? validationContext.DisplayName;
        return new ValidationResult(
            FormatErrorMessage(validationContext.DisplayName),
            memberName is not null ? [memberName] : null);
    }
}
