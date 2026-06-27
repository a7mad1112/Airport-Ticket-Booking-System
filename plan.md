# Airport Ticket Booking System - Architecture Plan

## 1. Architectural Approach
To ensure the code is readable, maintainable, and properly split, this project will utilize an N-Tier / Clean Architecture structure. This approach isolates the core business logic from data access and the user interface.

### 1.1. Layer Breakdown
* **Domain Layer (`AirportTicketBooking.Core`):** Contains enterprise logic and fundamental types.
    * **Entities:** `Flight`, `Booking`, `Passenger`, `Manager`.
    * **Enums:** `FlightClass` (Economy, Business, FirstClass), `BookingStatus`.
    * **Validation Attributes:** DataAnnotations that define the schema for manager validations.
* **Application Layer (`AirportTicketBooking.Application`):** Contains the business use cases and interfaces.
    * **Services:** `FlightSearchService`, `BookingManagementService`, and `BatchUploadService`.
    * **Interfaces:** `IFlightService`, `IBookingService`, `IFlightRepository`, `IBookingRepository`.
* **Infrastructure Layer (`AirportTicketBooking.Infrastructure`):** Implementation of data access and external libraries.
    * **Repositories:** `FileFlightRepository` and `FileBookingRepository` using `System.Text.Json`.
    * **Parsers:** Implements `CsvHelper` for manager uploads.
* **Presentation Layer (`AirportTicketBooking.UI`):** The console application entry point.
    * Utilizes Dependency Injection.
    * Static methods to cleanly format console output like tables and error messages.

📁 AirportTicketBooking.sln
│
├── 📁 AirportTicketBooking.Core          (Class Library)
│   ├── 📁 Entities                       // Flight, Booking, User
│   ├── 📁 Enums                          // FlightClass, BookingStatus
│   └── 📁 Exceptions                     // Custom Domain Exceptions
│
├── 📁 AirportTicketBooking.Application   (Class Library)
│   ├── 📁 Interfaces                     // IFlightService, IFlightRepository
│   ├── 📁 Services                       // FlightSearchService, BookingService
│   └── 📁 DTOs                           // Data Transfer Objects
│
├── 📁 AirportTicketBooking.Infrastructure (Class Library)
│   ├── 📁 Repositories                   // FileFlightRepository (JSON reading/writing)
│   └── 📁 Parsers                        // CsvFlightImportAdapter (CsvHelper)
│
└── 📁 AirportTicketBooking.UI            (Console Application)
    ├── 📁 Controllers                    // Menus and User Prompts
    ├── 📁 Views                          // Console Table Formatting
    └── 📄 Program.cs                     // Entry point & Dependency Injection Setup


### 1.2. Dependency Graph
This diagram illustrates the direction of dependencies between the class libraries and the console application. In Clean Architecture, all inner logic points toward the Core, while the UI wires everything together via Dependency Injection.

       ┌────────────────────────────────┐
       │   Presentation Layer (UI)      │
       └──────┬──────────────────┬──────┘
              │                  │
              ▼                  ▼
┌─────────────────┐       ┌──────────────────────┐
│ Infrastructure  │       │     Application      │
└──────┬──────────┘       └──────────┬───────────┘
       │                             │
       │       ┌─────────────┐       │
       └──────►│    Core     │◄──────┘
               └─────────────┘

---

## 2. Data Storage Strategy
Databases are not allowed for this project. The local file system will be used as the transactional data storage layer:
* **`data/flights.json`:** Serves as the single source of truth for all flights.
* **`data/bookings.json`:** Stores all user bookings.
* **`imports/`:** A directory designated for managers to drop `.csv` files for batch processing.

---

## 3. Passenger Features Implementation
* **Pricing by Class:** Implement a strategy pattern or domain multiplier for booking a flight.
    * Economy: BasePrice * 1.0m.
    * Business: BasePrice * 1.5m.
    * First Class: BasePrice * 2.5m.
* **Complex Search & Filtering:** Create a `FlightSearchCriteria` class utilizing a Builder Pattern or Fluent LINQ Extensions. Standard `IQueryable` or `IEnumerable` extension methods will chain filters dynamically to search by Price, Country, Date, Airports, and Class.
* **Booking Management:** Dedicated methods to manage bookings, including Cancel, Modify, and View operations.

---

## 4. Manager Features Implementation
* **Booking Filtering:** Utilize the same dynamic LINQ extensions used in the passenger search to filter bookings by various flight and passenger parameters.
* **Batch Flight Upload & Validation:** Use `CsvHelper` and `System.ComponentModel.DataAnnotations`.
    1.  Parse the CSV into memory.
    2.  Iterate through each parsed `Flight` DTO.
    3.  Use `Validator.TryValidateObject` to apply model-level validation.
    4.  Collect all `ValidationResult` objects and return a grouped detailed error dictionary to the UI if errors exist. Otherwise, persist to `flights.json`.
* **Dynamic Model Validation Details:** Use C# Reflection to provide dynamically generated validation constraints.
    1.  Create a `MetadataService`.
    2.  Use `typeof(Flight).GetProperties()` to iterate over fields.
    3.  Call `property.GetCustomAttributes(true)` to extract constraints like `RequiredAttribute` or `RangeAttribute`.
    4.  Map these attributes to readable strings for the UI.

---

## 5. Recommended Tech Stack
* **Framework:** .NET 8.0 (LTS) Console Application.
* **Dependency Injection:** `Microsoft.Extensions.DependencyInjection`.
* **CSV Parsing:** `CsvHelper`.
* **Serialization:** `System.Text.Json`.

---

## 6. Implementation Phase Roadmap
* **Phase 1: Foundation & Domain:** Setup the solution structure, define core entities, and annotate entities with Data Annotations for validation.
* **Phase 2: Infrastructure & Data Access:** Implement generic file reading/writing utilities, repositories, and seed initial data into `flights.json`.
* **Phase 3: Application Logic:** Implement the `FlightService` and `BookingService`, along with the C# Reflection utility for dynamic validation details.
* **Phase 4: Batch Processing:** Implement `CsvHelper` configurations and the validation pipeline to return detailed row-by-row error logs.
* **Phase 5: Presentation & Polish:** Build the interactive console menus, wire up Dependency Injection, and conduct end-to-end testing.