using System.ComponentModel.DataAnnotations;
using AirportTicketBooking.Application.Interfaces;
using AirportTicketBooking.Core.Entities;
using AirportTicketBooking.Core.Validation;

namespace AirportTicketBooking.Application.Services;

public sealed class MetadataService : IMetadataService
{
    public Dictionary<string, List<string>> GetFlightValidationRules()
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var property in typeof(Flight).GetProperties())
        {
            var constraints = new List<string>();

            foreach (var attr in property.GetCustomAttributes(inherit: true))
            {
                var constraint = attr switch
                {
                    RequiredAttribute             => "Required",
                    RangeAttribute range          => $"Range: {range.Minimum} to {range.Maximum}",
                    StringLengthAttribute len     => BuildStringLengthConstraint(len),
                    EmailAddressAttribute         => "Must be a valid email address",
                    FutureDateAttribute           => "Must be a future date (today → future)",
                    DataTypeAttribute dt          => $"Data type: {dt.DataType}",
                    _                             => null
                };

                if (constraint is not null)
                    constraints.Add(constraint);
            }

            if (constraints.Count == 0)
                continue;

            // Prefer the [Display(Name = "...")] value as the human-readable key;
            // fall back to the raw property name when no Display attribute is present.
            var displayAttr = property
                .GetCustomAttributes(typeof(DisplayAttribute), inherit: true)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            var key = displayAttr?.Name ?? property.Name;
            result[key] = constraints;
        }

        return result;
    }

    private static string BuildStringLengthConstraint(StringLengthAttribute attr) =>
        attr.MinimumLength > 0
            ? $"Length: {attr.MinimumLength} to {attr.MaximumLength} characters"
            : $"Max length: {attr.MaximumLength} characters";
}
