using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{

    /// <summary>
    /// Controller for managing sales operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of SalesController
        /// </summary>
        /// <param name="mediator">The mediator instance</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new sale
        /// </summary>
        /// <param name="request">The sale creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors
                                 .Select(e => (ValidationErrorDetail)e)
                });


            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);
            var response = _mapper.Map<CreateSaleResponse>(result);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = response
            });
        }

        /// <summary>
        /// Retrieves a sale by its ID
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaleById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetSaleRequest { Id = id };
            var validator = new GetSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => (ValidationErrorDetail)e)
                });

            var query = _mapper.Map<GetSaleQuery>(request);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found"
                });

            var response = _mapper.Map<GetSaleResponse>(result);

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = response
            });
        }

        /// <summary>
        /// Retrieves paginated list of sales
        /// </summary>
        /// <param name="request">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of sales</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSalesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSales([FromQuery] GetSalesRequest request, CancellationToken cancellationToken)
        {
            var validator = new GetSalesRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => (ValidationErrorDetail)e)
                });

            var query = _mapper.Map<GetSalesQuery>(request);
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<GetSalesResponse>(result);

            return Ok(new ApiResponseWithData<GetSalesResponse>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = response
            });
        }

        /// <summary>
        /// Updates an existing sale by ID
        /// </summary>
        /// <param name="request">The updated sale request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated sale details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "ID in URL and request body do not match."
                });

            var validator = new UpdateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = validationResult.Errors.Select(f => (ValidationErrorDetail)f)
                });
            }

            var command = _mapper.Map<UpdateSaleCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found."
                });
            }

            return Ok(new ApiResponseWithData<UpdateSaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = _mapper.Map<UpdateSaleResponse>(result)
            });
        }

        /// <summary>
        /// Deletes a sale by its ID
        /// </summary>
        /// <param name="id">The ID of the sale to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success response or not found</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteSaleCommand(id);
            var success = await _mediator.Send(command, cancellationToken);

            if (!success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Sale deleted successfully."
            });
        }


    }
}