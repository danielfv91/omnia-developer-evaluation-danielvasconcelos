using System.Net;
using System.Text.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Middleware to handle unhandled exceptions globally.
/// Logs the exception and returns a generic error response.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly ILogger _logger = Log.ForContext("SourceContext", "ExceptionMiddleware");

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
