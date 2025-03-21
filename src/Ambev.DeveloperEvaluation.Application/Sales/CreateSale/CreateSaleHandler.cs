using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{

    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = _mapper.Map<Sale>(request);
            sale.Id = Guid.NewGuid();
            sale.IsCancelled = false;

            foreach (var item in sale.Items)
            {
                item.Id = Guid.NewGuid();
                item.DiscountPercentage = CalculateDiscount(item.Quantity);
                item.TotalItemAmount = CalculateTotalItem(item.Quantity, item.UnitPrice, item.DiscountPercentage);
                item.IsCancelled = false;
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalItemAmount);

            await _saleRepository.AddAsync(sale);

            return new CreateSaleResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                TotalAmount = sale.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };
        }

        private decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 20;
            if (quantity >= 4 && quantity < 10) return 10;
            return 0;
        }

        private decimal CalculateTotalItem(int quantity, decimal unitPrice, decimal discount)
        {
            var total = quantity * unitPrice;
            return total - (total * discount / 100);
        }
    }
}