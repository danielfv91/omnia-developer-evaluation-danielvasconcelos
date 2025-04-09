# Changelog — Ambev Developer Evaluation Project

## v1.3.0 — Full Automated Tests + Events + Logs + Best Practices

### Overview
This release represents the final and consolidated delivery of the project, focused on code quality, automated tests, structured logging, and complete documentation.

### Delivered Features

- Full CRUD for sales
- Business rules applied:
  - 4–9 items: 10% discount
  - 10–20 items: 20% discount
  - More than 20: not allowed
- Standardized API responses (ApiResponse)
- Validation with FluentValidation and English messages

### Delivered Enhancements

- Domain Events:
  - SaleCreatedEvent
  - SaleModifiedEvent
  - SaleCancelledEvent
  - ItemCancelledEvent
- Decoupled publisher via `IEventPublisher`
- Structured event logging (event-log-*.txt)

### Automated Tests

- Unit tests covering all business rules
- Validation tests with FluentValidation
- Functional tests with real HTTP calls and Testcontainers
- Integration tests using MediatR + PostgreSQL + Validation Pipeline

### Best Practices and Architecture

- SOLID, DRY, and YAGNI principles applied
- Removed duplicated logic from Handlers
- Created `ISaleItemBuilder` with single responsibility
- Clear separation between WebApi and Application layers
- Global exception handling middleware

### Documentation

- Updated README in English and Portuguese
- Added Documentation:
  - Project Architecture
  - Testing Strategy
  - Logs and Events
  - Clean Code and Best Practices

---

## Version History

### v1.2.1 — Test Refactor and Feature-Based Organization

- Complete test refactor for Sales feature
- Created SaleFakerBuilder using Bogus
- Reorganized test folders by feature
- Maintained full API compatibility

### v1.2.0 — FluentValidation Pipeline + Architecture Docs

- Automatic validation with FluentValidation in MediatR pipeline
- Cleaner controller code
- Added technical architecture documentation (pt/en)

### v1.1.1 — Authentication Fix + README update

- Fixed AutoMapper configuration for JWT auth
- Updated README with authentication instructions

### v1.1.0 — Final Delivery with Domain Events and Docs

- Domain events implemented with unit tests
- Fully detailed bilingual documentation

### v1.0.0 — Initial Delivery (CRUD Only)

- Complete sales CRUD implementation
- Business rule coverage with discounts
- Unit tests
- DDD architecture and clean structure