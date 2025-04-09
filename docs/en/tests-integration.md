# Integration Tests

This document describes the coverage, structure, and tools used in the integration tests for the project.

---

## Purpose of Integration Tests

Ensure that the core `Sales` domain components interact correctly with:

- The real database (PostgreSQL via Testcontainers)
- The validation pipeline (FluentValidation + MediatR)
- Persistence and read behaviors

---

## Technologies used

- **xUnit**: test framework
- **FluentAssertions**: expressive assertions
- **FluentValidation**: input and business validation
- **MediatR**: orchestration of commands and queries
- **AutoMapper**: entity-to-DTO mapping
- **Testcontainers for .NET**: real PostgreSQL containerized for isolation
- **Bogus**: realistic data generation
- **NSubstitute**: mocking for internal service contracts (when needed)

---

## Test Scope

### Handlers covered

| Handler                | Scope                                      |
|------------------------|--------------------------------------------|
| `CreateSaleHandler`    | Persistence, events, discount rules        |
| `UpdateSaleHandler`    | Update flow, conditional discounts, events |
| `GetSaleHandler`       | Fetch by ID with input validation          |
| `GetSalesHandler`      | Pagination, sorting, filtering, validation |
| `DeleteSaleHandler`    | Logical cancellation + event publishing    |
| `SaleLifecycleHandler` | Full flow: create → read → update → delete |

---

## Validations covered

Domain rules are validated **through the MediatR pipeline**, ensuring the same behavior as the real application.

Examples include:
- Required fields (e.g., `CustomerName`, `SaleNumber`)
- Quantity limits (`Quantity` between 1 and 20)
- Correct application of discounts based on quantity
- Rejection of invalid filters and sort parameters in `GetSales`

---

## Test Execution Flow

- On startup, a PostgreSQL container is created using port `5533`
- The database is migrated using EF Core Migrations
- Test data is inserted and read using the actual repository implementation
- The container is disposed of automatically at the end

---

## Test Structure

```bash
tests/
└── Ambev.DeveloperEvaluation.Integration/
    ├── Common/
    │   └── IntegrationTestFixture.cs
    ├── Builders/
    │   ├── SaleCommandFakerBuilder.cs
    │   └── SaleUpdateFakerBuilder.cs
    └── Handlers/
        ├── CreateSaleHandlerTests.cs
        ├── UpdateSaleHandlerTests.cs
        ├── GetSaleHandlerTests.cs
        ├── GetSalesHandlerTests.cs
        ├── SaleLifecycleHandlerTests.cs
        └── DeleteSaleHandlerTests.cs
