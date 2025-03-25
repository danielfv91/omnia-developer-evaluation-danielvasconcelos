# Testing Strategy

The project includes unit tests for all business logic, ensuring behavior correctness and safety during changes.

## Tools Used

- xUnit
- NSubstitute
- Bogus (for test data generation)
- AutoMapper configuration

## Test Coverage

| Handler                | Coverage                             |
|------------------------|--------------------------------------|
| CreateSaleHandler      | Business logic + discount rules      |
| UpdateSaleHandler      | Business logic + discount rules      |
| DeleteSaleHandler      | Deletion behavior                    |
| Event publishing tests | SaleCreated, SaleModified, Cancelled |

Tests are located in the `tests/Ambev.DeveloperEvaluation.Unit` project.