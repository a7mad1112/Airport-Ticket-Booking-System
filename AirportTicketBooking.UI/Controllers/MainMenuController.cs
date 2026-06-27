using AirportTicketBooking.UI.Views;

namespace AirportTicketBooking.UI.Controllers;

public sealed class MainMenuController
{
    private readonly PassengerController _passengerController;
    private readonly ManagerController _managerController;

    public MainMenuController(
        PassengerController passengerController,
        ManagerController managerController)
    {
        _passengerController = passengerController;
        _managerController   = managerController;
    }

    public async Task StartAsync()
    {
        PrintBanner();

        while (true)
        {
            PrintMenu();
            var choice = ConsoleHelper.PromptMenuChoice(2);

            switch (choice)
            {
                case 1: await _passengerController.RunAsync(); break;
                case 2: await _managerController.RunAsync();   break;
                case 0:
                    ConsoleHelper.PrintInfo("Goodbye!");
                    return;
            }
        }
    }

    private static void PrintBanner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║          ✈  Airport Ticket Booking System  ✈            ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    private static void PrintMenu()
    {
        ConsoleHelper.PrintSectionHeader("Main Menu");
        Console.WriteLine("  1. Passenger");
        Console.WriteLine("  2. Manager");
        Console.WriteLine("  0. Exit");
        ConsoleHelper.PrintSectionFooter();
    }
}
