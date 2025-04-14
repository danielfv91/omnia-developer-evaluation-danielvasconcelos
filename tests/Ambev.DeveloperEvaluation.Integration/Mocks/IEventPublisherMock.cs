using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Integration.Mocks;

public class IEventPublisherMock : IEventPublisher
{
    public List<object> PublishedEvents { get; } = new();

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        PublishedEvents.Add(@event!);
        return Task.CompletedTask;
    }

    public void Reset() => PublishedEvents.Clear();
}
