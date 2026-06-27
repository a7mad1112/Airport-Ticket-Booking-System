namespace AirportTicketBooking.Application.Interfaces;

public interface IMetadataService
{
    // Key = display name of the property; Value = list of human-readable constraint strings.
    Dictionary<string, List<string>> GetFlightValidationRules();
}
