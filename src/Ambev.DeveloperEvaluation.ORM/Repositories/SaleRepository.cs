using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
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

        public async Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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
                return;

            _context.SaleItems.RemoveRange(existingSale.Items);

            foreach (var item in sale.Items)
            {
                item.Id = Guid.NewGuid();
                item.SaleId = sale.Id;
                _context.SaleItems.Add(item);
            }

            existingSale.SaleNumber = sale.SaleNumber;
            existingSale.SaleDate = sale.SaleDate;
            existingSale.CustomerId = sale.CustomerId;
            existingSale.CustomerName = sale.CustomerName;
            existingSale.Branch = sale.Branch;
            existingSale.TotalAmount = sale.TotalAmount;
            existingSale.IsCancelled = sale.IsCancelled;

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
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(branch))
                query = query.Where(s => s.Branch.Contains(branch));

            if (minDate.HasValue)
                query = query.Where(s => s.SaleDate >= minDate.Value);

            if (maxDate.HasValue)
                query = query.Where(s => s.SaleDate <= maxDate.Value);

            var totalItems = await query.CountAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(order))
            {
                var orderParams = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                bool firstOrder = true;

                foreach (var param in orderParams)
                {
                    var trimmed = param.Trim();
                    var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var property = parts[0];
                    var direction = parts.Length > 1 ? parts[1].ToLower() : "asc";

                    var orderExpression = $"{property} {direction}";

                    query = firstOrder
                        ? query.OrderBy(orderExpression)
                        : ((IOrderedQueryable<Sale>)query).ThenBy(orderExpression);

                    firstOrder = false;
                }
            }
            else
            {
                query = query.OrderByDescending(s => s.SaleDate);
            }

            var sales = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return (sales, totalItems);
        }
    }
}
