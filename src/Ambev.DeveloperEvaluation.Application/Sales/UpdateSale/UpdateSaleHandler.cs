using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
                return null;

            sale.SaleNumber = request.SaleNumber;
            sale.SaleDate = request.SaleDate;
            sale.CustomerId = request.CustomerId;
            sale.CustomerName = request.CustomerName;
            sale.Branch = request.Branch;

            var existingItems = sale.Items ?? new List<SaleItem>();
            var updatedProductIds = request.Items.Select(i => i.ProductId).ToHashSet();

            var cancelledItems = existingItems
                .Where(item => !updatedProductIds.Contains(item.ProductId))
                .ToList();

            foreach (var cancelledItem in cancelledItems)
            {
                var itemCancelledEvent = new ItemCancelledEvent
                {
                    SaleId = sale.Id,
                    ItemId = cancelledItem.Id,
                    ProductId = cancelledItem.ProductId,
                    ProductName = cancelledItem.ProductName,
                    CancelledAt = DateTime.UtcNow,
                    Reason = "Item removed during sale update"
                };

                await _eventPublisher.PublishAsync(itemCancelledEvent, cancellationToken);
            }

            sale.Items = new List<SaleItem>();

            foreach (var item in request.Items)
            {
                var discount = CalculateDiscount(item.Quantity);
                var saleItem = new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = CalculateTotalItem(item.Quantity, item.UnitPrice, discount),
                    IsCancelled = false
                };

                sale.Items.Add(saleItem);
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalItemAmount);

            await _saleRepository.UpdateAsync(sale);

            var saleModifiedEvent = new SaleModifiedEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                TotalAmount = sale.TotalAmount
            };

            await _eventPublisher.PublishAsync(saleModifiedEvent, cancellationToken);

            return _mapper.Map<UpdateSaleResult>(sale);
        }

        private decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 20;
            if (quantity >= 4 && quantity < 10) return 10;
            if (quantity > 20)
                throw new BusinessException("Cannot sell more than 20 identical items per product.");
            return 0;
        }

        private decimal CalculateTotalItem(int quantity, decimal unitPrice, decimal discount)
        {
            var total = quantity * unitPrice;
            return total - (total * discount / 100);
        }
    }
}
