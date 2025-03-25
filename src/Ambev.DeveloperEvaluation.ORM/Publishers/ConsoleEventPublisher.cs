using Ambev.DeveloperEvaluation.Application.Events;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Infrastructure.Publishers
{
    public class ConsoleEventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T @event) where T : class
        {
            var eventType = typeof(T).Name;
            var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($"[Event Published] {eventType}:\n{json}");
            return Task.CompletedTask;
        }
    }
}