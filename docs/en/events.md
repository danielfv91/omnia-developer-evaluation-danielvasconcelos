
# Event Publishing in the Project

## Purpose

The event publishing mechanism allows different parts of the system to react to key actions without tight coupling. This aligns with the principles of Clean Architecture and Domain-Driven Design (DDD), especially for separation of concerns and scalability.

## Event Location

All events are located under the following namespace and structure:

```
src/
└── Application/
    └── Events/
        ├── Interfaces/
            ├── ISaleEvent.cs
        │   └── IEventPublisher.cs
        └── Sales/
            ├── SaleCreatedEvent.cs
            ├── SaleModifiedEvent.cs
            ├── SaleCancelledEvent.cs
            └── ItemCancelledEvent.cs
        └── EventPublisher.cs
```

## Key Interfaces

### `IEventPublisher`

```csharp
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
```

## Event Implementations

All events implement `ISaleEvent`, which includes common fields like:

```csharp
public interface ISaleEvent
{
    Guid SaleId { get; }
    DateTime Timestamp { get; }
}
```

### Types of Events

- **SaleCreatedEvent** – Published when a new sale is created.
- **SaleModifiedEvent** – Published when a sale is updated.
- **SaleCancelledEvent** – Published when a sale is deleted.
- **ItemCancelledEvent** – Published when sale items are replaced on update.

## Event Publisher

Implemented in:

```
Application/Events/EventPublisher.cs
```

Logs every event with Serilog using a custom `SourceContext`:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "EventPublisher");
_logger.Information("Event published: {@Event}", @event);
```

## Log Separation with Serilog

Configured in `program.cs` to write all events to:

```
logs/event-log-{Date}.txt
```

## Event Triggers

- `CreateSaleHandler`: publishes `SaleCreatedEvent`
- `UpdateSaleHandler`: publishes `SaleModifiedEvent` and `ItemCancelledEvent`
- `DeleteSaleHandler`: publishes `SaleCancelledEvent`

## Testing

Event publishing is covered in:

```
tests/Unit/Application/Sales/EventPublishingTests.cs
```

Verifies that events are correctly published when operations are handled.

## Extending Events

To add new events:

1. Create the event model in a new or existing subfolder of `Events/`.
2. Implement `IEventPublisher.PublishAsync(...)` where necessary.
3. Add test coverage.

## Benefits

- Loose coupling
- Better observability (via Serilog)
- Readability and maintainability
- Supports future integrations (e.g., outbox pattern, message queues)
