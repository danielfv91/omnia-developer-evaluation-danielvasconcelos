using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
        {
            RuleFor(s => s.SaleNumber).GreaterThan(0);
            RuleFor(s => s.CustomerId).NotEmpty();
            RuleFor(s => s.CustomerName).NotEmpty().MaximumLength(100);
            RuleFor(s => s.Branch).NotEmpty().MaximumLength(50);

            RuleForEach(s => s.Items).SetValidator(new CreateSaleItemValidator());
        }
    }

    public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemDto>
    {
        public CreateSaleItemValidator()
        {
            RuleFor(i => i.ProductId).NotEmpty();
            RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            RuleFor(i => i.Quantity).InclusiveBetween(1, 20);
            RuleFor(i => i.UnitPrice).GreaterThan(0);
        }
    }
}
