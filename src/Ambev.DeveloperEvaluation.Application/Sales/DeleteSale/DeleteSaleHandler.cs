using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;

        public DeleteSaleHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
        {
            _saleRepository = saleRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var existingSale = await _saleRepository.GetByIdAsync(request.Id);

            if (existingSale == null)
                return false;

            await _saleRepository.DeleteAsync(request.Id, cancellationToken);

            var saleCancelledEvent = new SaleCancelledEvent
            {
                SaleId = request.Id,
                Reason = "Deleted via API"
            };

            await _eventPublisher.PublishAsync(saleCancelledEvent, cancellationToken);

            return true;
        }
    }
}
