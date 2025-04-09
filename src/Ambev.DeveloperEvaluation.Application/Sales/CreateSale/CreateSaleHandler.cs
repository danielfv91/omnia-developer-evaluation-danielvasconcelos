using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISaleItemBuilder _saleItemBuilder;

        public CreateSaleHandler(
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

        public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = request.SaleNumber,
                SaleDate = request.SaleDate,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                Branch = request.Branch
            };

            sale.Items = _saleItemBuilder.Build(request.Items, sale.Id);
            sale.TotalAmount = _saleItemBuilder.CalculateTotalAmount(sale.Items);

            await _saleRepository.AddAsync(sale, cancellationToken);

            var createdEvent = new SaleCreatedEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                CustomerName = sale.CustomerName,
                TotalAmount = sale.TotalAmount,
                Timestamp = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(createdEvent, cancellationToken);

            return _mapper.Map<CreateSaleResult>(sale);
        }
    }
}
