using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    public class GetSalesRequest
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "SaleDate desc";
        public string? Branch { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}