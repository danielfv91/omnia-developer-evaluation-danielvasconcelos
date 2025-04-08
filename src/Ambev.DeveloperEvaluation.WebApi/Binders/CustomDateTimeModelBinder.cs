using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Ambev.DeveloperEvaluation.WebApi.Binders
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        private static readonly string[] _allowedFormats = new[]
        {
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy/MM/dd"
        };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            if (DateTime.TryParseExact(
                value,
                _allowedFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsedDate))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedDate);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Invalid date format. Use 'yyyy-MM-dd'. Example: 2025-04-03");
            return Task.CompletedTask;
        }
    }
}
