namespace Ambev.DeveloperEvaluation.Domain.Interfaces
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<object> DomainEvents { get; }
        void ClearDomainEvents();
    }

}
