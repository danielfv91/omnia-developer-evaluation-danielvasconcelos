using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existingSale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == sale.Id, cancellationToken);

        if (existingSale == null)
            throw new InvalidOperationException($"Sale with ID {sale.Id} not found.");

        _context.SaleItems.RemoveRange(existingSale.Items);

        foreach (var item in sale.Items)
        {
            item.Id = Guid.NewGuid();
            item.SaleId = sale.Id;
            _context.SaleItems.Add(item);
        }

        _context.Entry(existingSale).CurrentValues.SetValues(sale);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
        if (sale is null) return;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<Sale> Sales, int TotalItems)> GetSalesPagedAsync(
        int page, int size, string order, string branch, DateTime? minDate, DateTime? maxDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Where(s => !s.IsCancelled)
            .AsQueryable();

        query = ApplyBranchFilter(query, branch);
        query = ApplyDateFilters(query, minDate, maxDate);
        query = ApplyOrdering(query, order);

        var totalItems = await query.CountAsync(cancellationToken);

        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, totalItems);
    }

    #region Helpers

    private static IQueryable<Sale> ApplyBranchFilter(IQueryable<Sale> source, string branch)
    {
        if (string.IsNullOrWhiteSpace(branch))
            return source;

        return source.Where(s => s.Branch.ToLower().Contains(branch.ToLower()));
    }

    private static IQueryable<Sale> ApplyDateFilters(IQueryable<Sale> source, DateTime? min, DateTime? max)
    {
        if (min.HasValue)
            source = source.Where(s => s.SaleDate >= min.Value);

        if (max.HasValue)
        {
            var endOfDay = max.Value.TimeOfDay == TimeSpan.Zero
                ? max.Value.Date.AddDays(1).AddTicks(-1)
                : max.Value;

            source = source.Where(s => s.SaleDate <= endOfDay);
        }

        return source;
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> source, string order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return source.OrderByDescending(s => s.SaleDate);

        var orderParams = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        bool isFirst = true;

        foreach (var param in orderParams)
        {
            var parts = param.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var property = parts[0];
            var direction = parts.Length > 1 ? parts[1].ToLower() : "asc";
            var orderExpr = $"{property} {direction}";

            source = isFirst
                ? source.OrderBy(orderExpr)
                : ((IOrderedQueryable<Sale>)source).ThenBy(orderExpr);

            isFirst = false;
        }

        return source;
    }

    #endregion
}
