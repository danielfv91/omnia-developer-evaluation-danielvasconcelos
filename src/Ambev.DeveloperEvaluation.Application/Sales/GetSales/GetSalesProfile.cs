using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetSalesProfile : Profile
    {
        public GetSalesProfile()
        {
            CreateMap<Sale, SaleListItemDto>();
        }
    }
}