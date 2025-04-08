using Ambev.DeveloperEvaluation.WebApi.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    public class GetSalesRequest
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "SaleDate desc";
        public string? Branch { get; set; }

        [ModelBinder(BinderType = typeof(CustomDateTimeModelBinder))]
        public DateTime? MinDate { get; set; }

        [ModelBinder(BinderType = typeof(CustomDateTimeModelBinder))]
        public DateTime? MaxDate { get; set; }
    }
}
