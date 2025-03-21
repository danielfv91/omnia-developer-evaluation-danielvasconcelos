using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class UpdateSaleRequestProfile : Profile
    {
        public UpdateSaleRequestProfile()
        {
            CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
            CreateMap<UpdateSaleItemRequest, UpdateSaleItemDto>();

            CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        }
    }
}