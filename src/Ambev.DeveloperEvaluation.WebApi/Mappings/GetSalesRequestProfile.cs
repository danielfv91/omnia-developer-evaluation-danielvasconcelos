using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class GetSalesRequestProfile : Profile
    {
        public GetSalesRequestProfile()
        {
            CreateMap<GetSalesRequest, GetSalesQuery>();
            CreateMap<GetSalesResult, GetSalesResponse>();
            CreateMap<SaleListItemDto, SaleListItemResponse>();
        }
    }
}