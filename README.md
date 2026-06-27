# Airport Ticket Booking System

A robust, interactive console-based application for managing airport flight bookings. This system is designed to handle passenger flight reservations and provide managers with administrative tools, including data filtering and bulk flight imports. It is built with a focus on maintainability, leveraging Clean Architecture principles and modern C# features.

---

## Key Features

### 🧑‍✈️ Passenger Features
* **Flight Search:** Search for available flights based on various criteria (e.g., price, departure/destination country, date, airport).
* **Tiered Pricing:** View and book flights across different classes (Economy, Business, First Class) with dynamically calculated pricing based on class multipliers.
* **Booking Management:** Create, view, and manage personal flight bookings.

### 💼 Manager Features
* **Advanced Filtering:** Filter and view all passenger bookings and available flights in the system.
* **Batch Flight Upload:** Seamlessly import new flights via CSV files.
* **Dynamic Validation:** The batch upload mechanism utilizes Reflection to enforce dynamic validation constraints (e.g., future dates, required fields, price ranges) based on entity attributes, providing detailed error reporting per row.

---

## Architecture & Tech Stack

This project strictly adheres to **Clean Architecture** to ensure separation of concerns and testability:
* **Core:** Contains domain entities, enums, and custom validation attributes.
* **Application:** Contains business logic, services, DTOs, and interface definitions.
* **Infrastructure:** Implements data access (JSON file storage) and third-party integrations (CSV parsing).
* **UI:** The console presentation layer handling user interaction and Dependency Injection setup.

### Tech Stack
* **Framework:** .NET 8 Console Application
* **Data Persistence:** `System.Text.Json` for thread-safe JSON file storage.
* **Data Import:** `CsvHelper` for robust CSV parsing and mapping.
* **Inversion of Control:** `Microsoft.Extensions.DependencyInjection` for managing service lifetimes and dependencies.

---

## Project Structure

```text
AirportTicketBooking
├── AirportTicketBooking.Core/           # Domain Entities, Enums, Validation Attributes
├── AirportTicketBooking.Application/    # Business Services, Interfaces, DTOs
├── AirportTicketBooking.Infrastructure/ # JSON Repositories, CSV Parsers
└── AirportTicketBooking.UI/             # Console Interface, Controllers, Dependency Injection
```

---

## Getting Started

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.

### How to Run

1. Clone the repository and navigate to the root directory.
2. Build and run the UI project using the .NET CLI:

   ```bash
   dotnet run --project AirportTicketBooking.UI
   ```

### 💡 Note on Data Seeding
No manual database or file setup is required! On the first run, the application's programmatic **Data Seeder** will automatically:
* Generate a `data/flights.json` file populated with realistic sample flights.
* Generate a `imports/sample_flights.csv` file containing both valid and intentionally invalid flight records for testing purposes.

---

## Usage Instructions

Upon launching the application, you will be presented with an interactive console menu. You can log in as either a **Passenger** or a **Manager**.

* **Navigation:** Follow the on-screen prompts to navigate through the various menus, search for flights, or view bookings.
* **Testing Batch Upload:** To test the Manager's batch upload feature, log in as a Manager, select the batch upload option, and provide the path to the automatically generated sample file: `imports/sample_flights.csv`. You will see the system successfully import valid rows while reporting specific validation errors for the intentionally invalid rows.
