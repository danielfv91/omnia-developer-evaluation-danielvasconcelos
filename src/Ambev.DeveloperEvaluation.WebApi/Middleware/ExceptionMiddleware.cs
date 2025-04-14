using System.Net;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using ILogger = Serilog.ILogger;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Middleware to handle unhandled exceptions globally.
/// Logs the exception and returns a generic error response.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    public ExceptionMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger.ForContext("SourceContext", "ExceptionMiddleware");
    }

    /// <summary>
    /// Handles the HTTP request pipeline and captures unhandled exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.Warning(ex, "Resource not found: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = ex.Errors.Select(error => new ValidationErrorDetail
                {
                    Error = error.PropertyName,
                    Detail = error.ErrorMessage
                })
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occurred while processing the request.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "An unexpected error occurred. Please try again later."
            };

            var responseBody = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(responseBody);
        }
    }
}
