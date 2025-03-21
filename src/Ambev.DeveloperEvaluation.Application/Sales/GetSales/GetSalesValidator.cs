using FluentValidation;
using System;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetSalesValidator : AbstractValidator<GetSalesQuery>
    {
        private static readonly string[] AllowedOrderFields =
        {
        "SaleNumber", "SaleDate", "CustomerName", "Branch", "TotalAmount"
    };

        public GetSalesValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.Size)
                .InclusiveBetween(1, 100)
                .WithMessage("Size must be between 1 and 100.");

            RuleFor(x => x.Order)
                .Must(BeValidOrder)
                .WithMessage("Order must contain valid fields and directions (asc, desc).");

            RuleFor(x => x.MinDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.MinDate.HasValue)
                .WithMessage("MinDate cannot be in the future.");

            RuleFor(x => x.MaxDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.MaxDate.HasValue)
                .WithMessage("MaxDate cannot be in the future.");
        }

        private bool BeValidOrder(string order)
        {
            if (string.IsNullOrWhiteSpace(order))
                return true;

            var orderParts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in orderParts)
            {
                var trimmed = part.Trim();
                var tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 0 || tokens.Length > 2)
                    return false;

                var field = tokens[0];
                var direction = tokens.Length == 2 ? tokens[1].ToLower() : "asc";

                if (!AllowedOrderFields.Contains(field) || (direction != "asc" && direction != "desc"))
                    return false;
            }

            return true;
        }
    }
}