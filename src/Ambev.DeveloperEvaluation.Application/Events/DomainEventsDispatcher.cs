using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Events;

public class DomainEventsDispatcher
{
    private readonly DbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public DomainEventsDispatcher(DbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = _context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            entity.ClearDomainEvents();
        }
    }
}
