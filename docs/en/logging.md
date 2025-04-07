# Application Logging

This document describes how the application implements logging using **Serilog**, including structured logs, context-based routing, and separate files for events and exceptions.

## General Configuration

Logging is configured in `Ambev.DeveloperEvaluation.Common.Logging.LoggingExtension`.

The application uses **Serilog**, with context enrichment, detailed exception logging, and contextual file routing.

It is initialized in `Program.cs` with:

```csharp
builder.AddDefaultLogging();
app.UseDefaultLogging();
```

## Log Files

Logs are generated in the `logs/` folder with:

- `app-log-<date>.txt`: general application log.
- `event-log-<date>.txt`: event-specific log (e.g., sales creation or update).
- `exception-log-<date>.txt`: log of unhandled exceptions via global middleware.

## Event Logging

To log events in `event-log-*.txt`, we use the `EventPublisher` with a `SourceContext`:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "EventPublisher");
_logger.Information("Event published: {@Event}", @event);
```

## Exception Logging

Unhandled exceptions are logged using the `ExceptionMiddleware`, also with `SourceContext`:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "ExceptionMiddleware");
_logger.Error(ex, "Unhandled exception");
```

This middleware ensures failures are logged and a generic response is returned to the client.

## Extensibility

The setup allows contextual log routing using `AddContextualLogs`, which defines multiple `WriteTo.Logger` blocks filtered by `SourceContext`.

Example:

```csharp
.AddContextualLogs(new Dictionary<string, string>
{
    ["EventPublisher"] = "event",
    ["ExceptionMiddleware"] = "exception"
});
```

## Summary

This solution provides:

- Structured, contextual logs
- Separate files for events and exceptions
- Request tracing with enrichment
- Centralized exception handling

This approach ensures traceability, observability, and future auditing and analysis.
