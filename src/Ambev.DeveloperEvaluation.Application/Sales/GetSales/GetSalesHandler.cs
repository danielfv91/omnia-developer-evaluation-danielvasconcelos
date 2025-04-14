using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetSalesResult> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var (sales, totalItems) = await _saleRepository.GetSalesPagedAsync(
            request.Page,
            request.Size,
            request.Order,
            request.Branch,
            request.MinDate,
            request.MaxDate,
            cancellationToken
        );

        return new GetSalesResult
        {
            Data = _mapper.Map<IEnumerable<SaleListItemDto>>(sales),
            TotalItems = totalItems,
            CurrentPage = request.Page,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.Size)
        };
    }
}
