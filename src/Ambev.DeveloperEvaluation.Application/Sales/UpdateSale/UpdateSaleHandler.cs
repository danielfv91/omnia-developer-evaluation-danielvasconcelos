using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISaleItemBuilder _saleItemBuilder;

        public UpdateSaleHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IEventPublisher eventPublisher,
            ISaleItemBuilder saleItemBuilder)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _saleItemBuilder = saleItemBuilder;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
                throw new NotFoundException($"Sale with ID {request.Id} was not found.");

            sale.SaleNumber = request.SaleNumber;
            sale.SaleDate = request.SaleDate;
            sale.CustomerId = request.CustomerId;
            sale.CustomerName = request.CustomerName;
            sale.Branch = request.Branch;

            await PublishItemCancelledEvents(sale.Items, request.Items, sale.Id, cancellationToken);

            var newItems = _saleItemBuilder.Build(request.Items, sale.Id);
            sale.Items = newItems;
            sale.TotalAmount = _saleItemBuilder.CalculateTotalAmount(newItems);

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            var updatedEvent = new SaleModifiedEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                TotalAmount = sale.TotalAmount,
                Timestamp = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(updatedEvent, cancellationToken);

            return _mapper.Map<UpdateSaleResult>(sale);
        }

        private async Task PublishItemCancelledEvents(
            List<SaleItem> existingItems,
            List<UpdateSaleItemDto> updatedItems,
            Guid saleId,
            CancellationToken cancellationToken)
        {
            if (existingItems == null || existingItems.Count == 0)
                return;

            var updatedProductIds = updatedItems.Select(i => i.ProductId).ToHashSet();

            var cancelledItems = existingItems
                .Where(item => !updatedProductIds.Contains(item.ProductId))
                .ToList();

            foreach (var item in cancelledItems)
            {
                var cancelEvent = new ItemCancelledEvent
                {
                    SaleId = saleId,
                    ItemId = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    CancelledAt = DateTime.UtcNow,
                    Reason = "Item removed during sale update"
                };

                await _eventPublisher.PublishAsync(cancelEvent, cancellationToken);
            }
        }
    }
}
