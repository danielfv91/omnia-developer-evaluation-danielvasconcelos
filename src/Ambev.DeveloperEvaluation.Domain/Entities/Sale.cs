using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Domain.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{

    public class Sale : IHasDomainEvents
    {
        #region Attributes

        public Guid Id { get; private set; }
        public int SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public string Branch { get; private set; }
        public bool IsCancelled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private readonly List<SaleItem> _items = new();
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        public decimal TotalAmount { get; private set; }

        #endregion

        private Sale() { }

        #region Behaviors

        public static Sale Create(int saleNumber,
            DateTime saleDate,
            Guid customerId,
            string customerName,
            string branch,
            IEnumerable<SaleItemInput> items)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = saleNumber,
                SaleDate = saleDate,
                CustomerId = customerId,
                CustomerName = customerName,
                Branch = branch,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in items)
            {
                sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            }

            sale.RecalculateTotal();

            sale.RaiseEvent(new SaleCreatedDomainEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                CustomerName = sale.CustomerName,
                TotalAmount = sale.TotalAmount,
                Timestamp = DateTime.UtcNow
            });

            return sale;
        }

        public void Update(int saleNumber,
            DateTime saleDate,
            Guid customerId,
            string customerName,
            string branch,
            IEnumerable<SaleItemInput> updatedItems)
        {
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            CustomerId = customerId;
            CustomerName = customerName;
            Branch = branch;

            var previousItems = _items.ToList();
            var previousProductIds = previousItems.Select(i => i.ProductId).ToHashSet();

            _items.Clear();

            foreach (var item in updatedItems)
            {
                AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            }

            RecalculateTotal();

            var updatedProductIds = _items.Select(i => i.ProductId).ToHashSet();
            var removedProductIds = previousProductIds.Except(updatedProductIds);

            foreach (var removedId in removedProductIds)
            {
                var removedItem = previousItems.FirstOrDefault(i => i.ProductId == removedId);
                if (removedItem != null)
                {
                    RaiseEvent(new ItemCancelledDomainEvent
                    {
                        SaleId = Id,
                        ItemId = removedItem.Id,
                        ProductId = removedItem.ProductId,
                        ProductName = removedItem.ProductName,
                        CancelledAt = DateTime.UtcNow
                    });
                }
            }

            RaiseEvent(new SaleModifiedDomainEvent
            {
                SaleId = Id,
                SaleNumber = SaleNumber,
                TotalAmount = TotalAmount,
                Timestamp = DateTime.UtcNow
            });

            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(string reason = "Sale cancelled")
        {
            IsCancelled = true;

            RaiseEvent(new SaleCancelledDomainEvent
            {
                SaleId = Id,
                Timestamp = DateTime.UtcNow,
                Reason = reason
            });
        }

        private void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            if (quantity > 20)
                throw new BusinessException("Cannot sell more than 20 identical items per product.");

            var discount = CalculateDiscount(quantity);
            var total = CalculateTotal(quantity, unitPrice, discount);

            _items.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = Id,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                DiscountPercentage = discount,
                TotalItemAmount = total,
                IsCancelled = false
            });
        }

        private void RecalculateTotal()
        {
            TotalAmount = _items.Sum(i => i.TotalItemAmount);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

        private void RaiseEvent(object domainEvent) => _domainEvents.Add(domainEvent);

        private static decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 20;
            if (quantity >= 4 && quantity < 10) return 10;
            return 0;
        }

        private static decimal CalculateTotal(int quantity, decimal unitPrice, decimal discount)
        {
            var total = quantity * unitPrice;
            return total - (total * discount / 100);
        }

        #endregion
    }

    public class SaleItem
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TotalItemAmount { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class SaleItemInput
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
}