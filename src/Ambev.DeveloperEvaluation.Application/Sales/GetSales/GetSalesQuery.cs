using MediatR;
using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetSalesQuery : IRequest<GetSalesResult>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "SaleDate desc";

        // Filtros opcionais
        public string Branch { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}