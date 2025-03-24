using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
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
            var sale = await _saleRepository.GetByIdAsync(request.Id);

            if (sale == null)
                return null;

            sale.SaleNumber = request.SaleNumber;
            sale.SaleDate = request.SaleDate;
            sale.CustomerId = request.CustomerId;
            sale.CustomerName = request.CustomerName;
            sale.Branch = request.Branch;

            sale.Items = new List<SaleItem>();

            foreach (var item in request.Items)
            {
                var saleItem = new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalItemAmount = CalculateDiscountedAmount(item.Quantity, item.UnitPrice),
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

            await _eventPublisher.PublishAsync(saleModifiedEvent);

            return _mapper.Map<UpdateSaleResult>(sale);
        }

        private decimal CalculateDiscountedAmount(int quantity, decimal unitPrice)
        {
            decimal discount = 0;

            if (quantity >= 10 && quantity <= 20)
                discount = 0.20m;
            else if (quantity >= 4 && quantity < 10)
                discount = 0.10m;

            if (quantity > 20)
                throw new BusinessException("Cannot sell more than 20 identical items per product.");

            var total = quantity * unitPrice;
            return total - (total * discount);
        }
    }
}