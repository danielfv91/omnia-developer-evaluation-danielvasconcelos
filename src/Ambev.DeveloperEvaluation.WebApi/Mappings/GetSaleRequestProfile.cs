using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{

    public class GetSaleRequestProfile : Profile
    {
        public GetSaleRequestProfile()
        {
            CreateMap<GetSaleRequest, GetSaleQuery>();
            CreateMap<GetSaleResult, GetSaleResponse>();
            CreateMap<GetSaleItemDto, GetSaleItemResponse>();
        }
    }
}