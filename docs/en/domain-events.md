# Domain Events

This project includes the implementation of domain events as a demonstration of extensibility and separation of concerns.

## Implemented Events

- `SaleCreatedEvent`
- `SaleModifiedEvent`
- `SaleCancelledEvent`
- `ItemCancelledEvent` (structure created, not used yet)

## Publisher

A simple `ConsoleEventPublisher` was created to simulate event publishing by logging events in the console.

## Where events are triggered

| Event              | Trigger                               |
|--------------------|---------------------------------------|
| SaleCreatedEvent   | After a successful POST /sales        |
| SaleModifiedEvent  | After a successful PUT /sales/{id}    |
| SaleCancelledEvent | After a successful DELETE /sales/{id} |

## Why this matters

This demonstrates how the application can evolve to a reactive architecture, integrating event-based systems or message brokers in the future.