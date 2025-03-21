
# Ambev Developer Evaluation Project

This guide will help you quickly set up the project on your local environment.

---

## Prerequisites

Before you start, make sure you have the following tools installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL](https://www.postgresql.org/download/) (PgAdmin included in installation is recommended)
- [Git](https://git-scm.com/downloads)

---

## Initial Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-user/your-repository.git
cd your-repository
```

### 2. Configure PostgreSQL Database

- Open PgAdmin and create a new database named `DeveloperEvaluation`.

### 3. Update the Connection String

In the `appsettings.json` file located at `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`, update the connection string with your PostgreSQL credentials:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=DeveloperEvaluation;User Id=postgres;Password=your_password;"
}
```

### 4. Apply EF Core Migrations

In Visual Studio's **Package Manager Console**, run:

```powershell
Update-Database -Project Ambev.DeveloperEvaluation.ORM -StartupProject Ambev.DeveloperEvaluation.WebApi
```

This will automatically create the required tables in your PostgreSQL database.

---

## ▶Running the Application

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
├── Ambev.DeveloperEvaluation.Application
├── Ambev.DeveloperEvaluation.Common
├── Ambev.DeveloperEvaluation.Crosscutting
├── Ambev.DeveloperEvaluation.Domain
├── Ambev.DeveloperEvaluation.Infrastructure
├── Ambev.DeveloperEvaluation.IoC
├── Ambev.DeveloperEvaluation.ORM
└── Ambev.DeveloperEvaluation.WebApi

tests/
├── Ambev.DeveloperEvaluation.Unit
├── Ambev.DeveloperEvaluation.Integration
└── Ambev.DeveloperEvaluation.Functional
```

---

## Tech Stack

- .NET 8
- Entity Framework Core
- PostgreSQL
- xUnit (tests)
- MediatR, AutoMapper, Rebus
- Git (version control)

---

**Now you're all set to start!**
