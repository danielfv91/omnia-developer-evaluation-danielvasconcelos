# Project Architecture Documentation

## 1. Introduction

The project purpose is to provide a complete RESTful API for managing sales, including business rules, unit testing, and a solid and extensible architecture based on Domain-Driven Design (DDD) principles.

---

## 2. Domain-Driven Design (DDD) Architecture

The project follows a layered organization based on DDD:

```
src/
├── Domain            → Entities and repository interfaces
├── Application       → Use cases (Handlers, Commands, Validators)
├── ORM               → EF Core, DbContext, repository implementations
├── WebApi            → Controllers, Requests, Responses, Middleware
├── Common/Crosscut   → Exceptions, validations, utilities
└── IoC               → Dependency injection configuration
```

### Layer Responsibilities:

- **Domain**: Core business logic (entities like `Sale`, `SaleItem`, and interfaces like `ISaleRepository`).
- **Application**: Orchestrates use cases via MediatR. Contains commands (`CreateSaleCommand`, etc), handlers, results, and validators.
- **ORM (Infrastructure)**: Persistence layer using EF Core. Includes `DefaultContext`, `SaleRepository`, and entity mappings.
- **WebApi**: Exposes endpoints, handles input/output via `Request` and `Response`, performs validation and handler execution.
- **Common/Crosscutting**: Contains shared concerns like exception handling (`BusinessException`), event publishing, and validation.
- **IoC**: Configures dependency injection for all layers.

---

## 3. Patterns Used

| Pattern               | Implementation                                             |
|-----------------------|------------------------------------------------------------|
| CQRS                  | MediatR handlers separate commands and queries             |
| Repository Pattern    | `ISaleRepository` / `SaleRepository`                       |
| AutoMapper            | DTO <-> Entity mapping                                     |
| FluentValidation      | Input validation with English messages                     |
| Domain Events         | Events like `SaleCreatedEvent` published via console log   |
| Middleware            | Global exception and validation handling                   |

---

## 4. Requirement Coverage

Direct mapping of the evaluation requirements to the implemented features:

| Requirement                                               | Implementation                                               |
|-----------------------------------------------------------|--------------------------------------------------------------|
| Complete CRUD for sales                                   | POST, GET, PUT, DELETE endpoints in `SalesController`        |
| SaleNumber, Customer, Branch, Products, Quantities, etc.  | Present in all requests/responses                            |
| TotalAmount per item and total sale                       | Automatically calculated with discounts applied              |
| Discount rules (4–9: 10%, 10–20: 20%)                     | Applied in Create and Update Handlers                        |
| Block sales with > 20 identical items                     | Validated via BusinessException (400 error)                  |
| Cancelled/Not Cancelled field                             | `IsCancelled` field available in the entity                  |
| Validation in English                                     | Handled with FluentValidation and enforced culture           |
| Unit Testing                                              | Covered with xUnit, NSubstitute, and Bogus                   |
| Optional Domain Events                                    | SaleCreated, SaleModified, SaleCancelled — all implemented   |

---

## 5. Automatic Request Validation (Improvement over Template)

In the original template, request validation was done manually in each controller using:

```csharp
var validator = new SomeRequestValidator();
var validationResult = await validator.ValidateAsync(request, cancellationToken);
```

To improve maintainability and scalability, this project adopts **automatic validation using FluentValidation’s pipeline integration**. This removes validation logic from the controllers and centralizes it at the framework level.

**What was added:**
- Configuration in `Program.cs`:
  ```csharp
  builder.Services.AddFluentValidationAutoValidation();
  builder.Services.AddValidatorsFromAssembly(typeof(ApplicationLayer).Assembly);
  builder.Services.AddValidatorsFromAssemblyContaining<Program>();
  ```
- All validators are automatically detected and executed by the pipeline.

**Benefit:** All incoming requests are validated before reaching the controller, and any validation failure is handled by the `ValidationExceptionMiddleware`, providing a consistent and clean error response.

---

## 6. Conclusion

The project is clean, scalable, and easy to maintain. The architectural patterns enable future extensions (e.g., message brokers), reuse of business logic, and reliable automated tests.