using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Serilog;

namespace Ambev.DeveloperEvaluation.ORM.Events;

public class EventPublisher : IEventPublisher
{
    private static readonly ILogger _logger = Log.ForContext("SourceContext", "EventPublisher");

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    where TEvent : class
    {
        _logger.Information("Event published: {@Event}", @event);
        return Task.CompletedTask;
    }

}
