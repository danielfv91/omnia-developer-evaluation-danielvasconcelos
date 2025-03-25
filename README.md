
# Ambev Developer Evaluation Project

## Project Overview
This project is an API for managing sales, developed in .NET 8 using PostgreSQL, MediatR, AutoMapper, and DDD architecture. It was created as part of a technical evaluation for a senior/specialist .NET developer position.

## Requirements — Fully Met

This API fully implements the required functionalities:

- [x] Complete CRUD for sales (Sales)
- [x] Business rules applied for item quantity-based discounts
- [x] All required fields present in operations (SaleNumber, Customer, Branch, Products, Quantities, etc.)
- [x] Data validation with standardized English messages
- [x] Domain events implemented and logged to the console (SaleCreated, SaleModified, SaleCancelled)

### Authentication

JWT authentication is fully functional in the project.

- The endpoint `/auth` returns a token after validating user credentials
- To protect any route (e.g., `/sales`), simply add the `[Authorize]` attribute

> Note: An AutoMapper configuration issue was found in the original project template and has been fixed to ensure correct JWT token generation.

## Features
- Create, retrieve, update, and delete sales records
- Business rules applied on sales items:
  - 4–9 items: 10% discount
  - 10–20 items: 20% discount
  - More than 20 items: not allowed
- Input validation with FluentValidation and English messages
- Consistent API response format (ApiResponse)
- Unit tests covering all business rule scenarios

## Technologies
- .NET 8.0
- PostgreSQL + EF Core
- MediatR (CQRS)
- AutoMapper
- xUnit + NSubstitute + Bogus (tests)
- FluentValidation
- Architecture: Domain-Driven Design (DDD)

## Project Setup

This guide will help you quickly set up the project on your local environment.

---

### Prerequisites

Before you start, make sure you have the following tools installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL](https://www.postgresql.org/download/) (PgAdmin included in installation is recommended)
- [Git](https://git-scm.com/downloads)

---

### Initial Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos.git
cd omnia-developer-evaluation-danielvasconcelos
```

#### 2. Configure PostgreSQL Database

- Open PgAdmin and create a new database named `DeveloperEvaluation`.

#### 3. Update the Connection String

In the `appsettings.json` file located at `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`, update the connection string with your PostgreSQL credentials:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=DeveloperEvaluation;User Id=postgres;Password=your_password;"
}
```

#### 4. Apply EF Core Migrations

In Visual Studio's **Package Manager Console**, run:

```powershell
Update-Database -Project Ambev.DeveloperEvaluation.ORM -StartupProject Ambev.DeveloperEvaluation.WebApi
```

This will automatically create the required tables in your PostgreSQL database.

---

### Running the Application

- In Visual Studio, set the `Ambev.DeveloperEvaluation.WebApi` project as the startup project and press **F5**.

- The application will start using HTTPS (`https://localhost:<port>`), opening the Swagger UI automatically.

### HTTPS Certificate Issue

If you encounter the error **"Your connection is not private"** (`ERR_CERT_INVALID`), execute the following commands in the command prompt (as administrator):

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Restart Visual Studio and run again.

---

## Running Tests

You can run tests directly from Visual Studio or through the terminal:

```bash
dotnet test
```

---

## Project Structure
```
src/
├── Application       → Business rules (MediatR Handlers, Commands)
├── Domain            → Entities and Interfaces (DDD)
├── ORM               → EF Core Context and Repositories
├── WebApi            → Controllers, Requests, Responses
├── Common/Crosscut   → Exceptions, Helpers, Validation
tests/
└── Unit              → Unit tests (xUnit, NSubstitute, Bogus)
```

## Commit Guidelines
Semantic commits in Portuguese:
- `feat:` new feature
- `fix:` bug fix
- `test:` tests
- `chore:` minor changes

## Repository Link
[github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos](https://github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos)

---

**Now you're all set to start!**