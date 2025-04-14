# Domain-Driven Design in the Project

This document outlines how the project applies the Rich Domain Model using Domain-Driven Design (DDD) principles.

## 1. Entities are the core of the domain

Entities (`Sale`, `SaleItem`) contain business rules such as:
- Discount and total calculation
- Quantity validation
- Item addition and cancellation

Example:

```csharp
if (quantity > 20)
    throw new BusinessException("Cannot sell more than 20 identical items per product.");
```

## 2. Domain Events

Events such as `SaleCreatedDomainEvent`, `SaleModifiedDomainEvent`, `SaleCancelledDomainEvent`, and `ItemCancelledDomainEvent` are raised directly from the domain. They are stored in a local collection (`DomainEvents`) and published later.

## 3. Decoupled event publishing

Publishing is performed by the `DomainEventsDispatcher` in the Application layer, using `IEventPublisher`. This allows flexible strategies (e.g., logging, messaging).

## 4. Handlers delegate to the domain

Handlers simply orchestrate the use case, deferring logic to the domain. Example:

```csharp
sale.Update(...); // Logic encapsulated in the domain
```

## 5. Persistence is isolated

The repository (`ISaleRepository`) works with domain entities and respects their state. Updates reflect changes made by the domain.
