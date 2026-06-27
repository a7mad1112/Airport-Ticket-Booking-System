using AirportTicketBooking.Core.Entities;

namespace AirportTicketBooking.UI.Views;

public static class ConsoleHelper
{
    private const int ColWidth = 20;

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ✓ {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ✗ {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
    }

    public static void PrintSectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ┌─ {title} {"".PadRight(Math.Max(0, 55 - title.Length), '─')}┐");
        Console.ResetColor();
    }

    public static void PrintSectionFooter()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  └{"".PadRight(59, '─')}┘");
        Console.ResetColor();
    }

    public static void PrintFlightsTable(IEnumerable<Flight> flights)
    {
        var list = flights.ToList();
        if (list.Count == 0)
        {
            PrintInfo("No flights match your criteria.");
            return;
        }

        PrintSectionHeader("Available Flights");
        PrintTableRow("#", "Flight", "From", "To", "Date", "Price(USD)", "Seats");
        PrintTableDivider();

        for (var i = 0; i < list.Count; i++)
        {
            var f = list[i];
            PrintTableRow(
                (i + 1).ToString(),
                f.FlightNumber,
                $"{f.DepartureAirport} ({f.DepartureCountry})",
                $"{f.ArrivalAirport} ({f.DestinationCountry})",
                f.DepartureDate.ToString("yyyy-MM-dd HH:mm"),
                $"${f.BasePrice:F2}",
                f.TotalSeats.ToString()
            );
        }

        PrintTableDivider();
        PrintSectionFooter();
    }

    public static void PrintBookingsTable(IEnumerable<Booking> bookings)
    {
        var list = bookings.ToList();
        if (list.Count == 0)
        {
            PrintInfo("No bookings found.");
            return;
        }

        PrintSectionHeader("Bookings");
        PrintTableRow("#", "Booking ID", "Flight ID", "Class", "Price Paid", "Status");
        PrintTableDivider();

        for (var i = 0; i < list.Count; i++)
        {
            var b = list[i];
            PrintTableRow(
                (i + 1).ToString(),
                b.Id.ToString()[..8] + "…",
                b.FlightId.ToString()[..8] + "…",
                b.Class.ToString(),
                $"${b.PricePaid:F2}",
                b.Status.ToString()
            );
        }

        PrintTableDivider();
        PrintSectionFooter();
    }

    public static void PrintValidationRules(Dictionary<string, List<string>> rules)
    {
        PrintSectionHeader("Flight Model Validation Rules");
        foreach (var (field, constraints) in rules)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  {field}:");
            Console.ResetColor();
            foreach (var constraint in constraints)
                Console.WriteLine($"    • {constraint}");
        }

        PrintSectionFooter();
    }

    public static void PrintImportErrors(Dictionary<int, List<string>> errors)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  Import failed — {errors.Count} row(s) contain errors:\n");
        Console.ResetColor();

        foreach (var (row, messages) in errors.OrderBy(e => e.Key))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  Row {row}:");
            Console.ResetColor();
            foreach (var msg in messages)
                Console.WriteLine($"    • {msg}");
        }
    }

    // Returns null if the user cancels (presses Enter with no input on an optional field).
    public static string? PromptOptional(string label)
    {
        Console.Write($"  {label} (Enter to skip): ");
        var value = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static string PromptRequired(string label)
    {
        string? value;
        do
        {
            Console.Write($"  {label}: ");
            value = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(value))
                PrintError("This field is required.");
        }
        while (string.IsNullOrEmpty(value));

        return value;
    }

    public static int PromptMenuChoice(int max)
    {
        while (true)
        {
            Console.Write("\n  Your choice: ");
            if (int.TryParse(Console.ReadLine(), out var choice) && choice >= 0 && choice <= max)
                return choice;

            PrintError($"Please enter a number between 0 and {max}.");
        }
    }

    public static bool PromptYesNo(string question)
    {
        Console.Write($"  {question} (y/n): ");
        return Console.ReadLine()?.Trim().ToLowerInvariant() == "y";
    }

    public static Guid? PromptGuid(string label)
    {
        Console.Write($"  {label}: ");
        var raw = Console.ReadLine()?.Trim();
        if (Guid.TryParse(raw, out var id))
            return id;

        PrintError("Invalid ID format.");
        return null;
    }

    public static decimal? PromptDecimal(string label)
    {
        Console.Write($"  {label} (Enter to skip): ");
        var raw = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(raw)) return null;
        if (decimal.TryParse(raw, out var val)) return val;
        PrintError("Invalid number.");
        return null;
    }

    public static DateTime? PromptDate(string label)
    {
        Console.Write($"  {label} yyyy-MM-dd (Enter to skip): ");
        var raw = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(raw)) return null;
        if (DateTime.TryParse(raw, out var date)) return date;
        PrintError("Invalid date format.");
        return null;
    }

    // ── Private table helpers ────────────────────────────────────────────────

    private static void PrintTableRow(params string[] cols)
    {
        var line = "  │";
        foreach (var col in cols)
        {
            var cell = col.Length > ColWidth ? col[..(ColWidth - 1)] + "…" : col;
            line += cell.PadRight(ColWidth) + "│";
        }
        Console.WriteLine(line);
    }

    private static void PrintTableDivider()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  ├" + string.Join("┼", Enumerable.Repeat("".PadRight(ColWidth, '─'), 7)) + "┤");
        Console.ResetColor();
    }
}
