# Testing Strategy

The project adopts a complete strategy of automated tests to ensure business logic correctness, system stability, and safety for future evolutions.

## Tools used

- **xUnit** — main testing framework
- **FluentAssertions** — expressive and readable assertions
- **NSubstitute** — mocking library
- **Bogus** — realistic test data generation
- **AutoMapper** — mapping configured for test scenarios

## Test structure

Tests are located under `tests/Ambev.DeveloperEvaluation.Unit` and are organized by type:

```
tests/
├── Application/
│   └── Sales/
│       ├── Handlers/         → MediatR Handlers tests
│       ├── Validators/       → FluentValidation rules tests
│       ├── Events/           → Events tests
│   └── TestData/             → Builders and centralized test data
```

## Handler test coverage

| Handler                | Coverage                                            |
|------------------------|-----------------------------------------------------|
| CreateSaleHandler      | Creation + discount rules                           |
| UpdateSaleHandler      | Update + discount + cancelled item events           |
| DeleteSaleHandler      | Deletion and event emission                         |
| GetSaleHandler         | Valid fetch and not found                           |
| GetSalesHandler        | Paging, filters and ordering                        |
| Event publishing       | SaleCreated, SaleModified, Cancelled, ItemCancelled |

## TestData strategy

Reusable and centralized test data is defined in:

- `SaleFakerBuilder` → Generates valid/realistic commands using Bogus
- `SalesHandlersTestData` → Provides combinations of quantity, price and expected discounts
- `SalesValidatorTestData` → Builds edge cases for validation scenarios

This reduces duplication and improves maintainability.
