using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{

    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
        {
            RuleFor(s => s.SaleNumber).GreaterThan(0);
            RuleFor(s => s.CustomerId).NotEmpty();
            RuleFor(s => s.CustomerName).NotEmpty().MaximumLength(100);
            RuleFor(s => s.Branch).NotEmpty().MaximumLength(50);

            RuleForEach(s => s.Items).SetValidator(new CreateSaleItemRequestValidator());
        }
    }

    public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
    {
        public CreateSaleItemRequestValidator()
        {
            RuleFor(i => i.ProductId).NotEmpty();
            RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            RuleFor(i => i.UnitPrice).GreaterThan(0);
        }
    }
}