using Ambev.DeveloperEvaluation.Domain.Entities;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Sale> Sales, int TotalItems)> GetSalesPagedAsync(
        int page, int size, string order, string branch,
        DateTime? minDate, DateTime? maxDate,
        CancellationToken cancellationToken = default);
}
