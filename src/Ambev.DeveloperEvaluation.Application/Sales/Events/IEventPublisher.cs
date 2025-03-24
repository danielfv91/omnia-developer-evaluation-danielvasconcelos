using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}