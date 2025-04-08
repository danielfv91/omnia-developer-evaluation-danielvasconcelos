
# Clean Code and Best Practices

## Overview

This document describes the decisions and improvements implemented in the project to ensure compliance with Clean Code principles and best architectural practices. Each section highlights how the criteria were met with objective justifications based on the code.

## SOLID Principles

### S - Single Responsibility Principle (SRP)
- Handlers (`CreateSaleHandler`, `UpdateSaleHandler`) were refactored to delegate item-building logic to the `SaleItemBuilder` class, centralizing behavior and respecting SRP.
- Clear separation between layers: controllers only coordinate flow, Application holds business logic, and Domain defines rules.

### O - Open/Closed Principle (OCP)
- Item construction and calculation logic is abstracted through the `ISaleItemBuilder` interface, enabling future extensions without modifying existing handlers.

### L - Liskov Substitution Principle (LSP)
- All dependencies are injected via interfaces, and substitution via mocks in tests ensures LSP compliance.

### I - Interface Segregation Principle (ISP)
- Interfaces like `ISaleItemBuilder` and `ISaleItemCalculator` are focused and have a single responsibility.

### D - Dependency Inversion Principle (DIP)
- Handlers and services rely on abstractions, not on concrete implementations.

## DRY

- Repeated item-building and discount logic extracted into `SaleItemBuilder`.
- Centralized test data using `SaleTestData` with Bogus.
- Clear boundary between validation responsibilities in WebApi and Application layers.

## YAGNI

- No speculative features were implemented.
- Removed unused files such as `IUserService.cs`.
- Minimalistic and focused implementations aligned with the challenge scope.

## Validations

- Validators by responsibility:
    UpdateSaleRequestValidator (WebApi): validates request format.
    UpdateSaleValidator (Application): validates business rules like item quantity limit.
- Ordering and date filters are validated in GetSalesValidator.
- ValidationExceptionMiddleware handles and returns validation errors with 400 status code.

## Events

- Events implemented:
    SaleCreatedEvent, SaleModifiedEvent, SaleCancelledEvent, ItemCancelledEvent.
- Publication is centralized via IEventPublisher.
- Tests confirm correct event publishing per scenario.

## Testing

- Unit tests leverage:
    Bogus for realistic data.
    NSubstitute for mocking.
    FluentAssertions for clear assertions.
- SaleTestData centralizes reusable scenarios.
- Coverage includes handlers, validators, and events.
- Exception middleware and validators to be tested in the dedicated test improvements branch.

## Clean Code

- Clear separation of concerns.
- Expressive and standardized naming.
- Structured layering: WebApi, Application, Domain, ORM, Common.
- No speculative logic or commented code.
- Uses REST conventions, AutoMapper, MediatR effectively.

## Additional Conformance

- Business rule validations implemented in `CreateSaleValidator` and `UpdateSaleValidator` (Application layer).
- Request integrity validations handled by `CreateSaleRequestValidator` and `UpdateSaleRequestValidator` (WebApi layer).
- Unit test coverage using FluentAssertions, Bogus, NSubstitute.
- Event publishing tested and refactored with proper responsibility separation.