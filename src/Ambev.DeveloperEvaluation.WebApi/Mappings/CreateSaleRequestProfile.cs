using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{

    public class CreateSaleRequestProfile : Profile
    {
        public CreateSaleRequestProfile()
        {
            // WebApi → Application
            CreateMap<CreateSaleRequest, CreateSaleCommand>();
            CreateMap<CreateSaleItemRequest, CreateSaleItemDto>();

            // Application → WebApi
            CreateMap<CreateSaleResult, CreateSaleResponse>();
        }
    }
}