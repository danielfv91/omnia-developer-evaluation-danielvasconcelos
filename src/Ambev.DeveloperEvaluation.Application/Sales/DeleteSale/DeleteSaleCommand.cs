using MediatR;
using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteSaleCommand(Guid id)
        {
            Id = id;
        }
    }
}