using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Middleware;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_HandleUnhandledException_AndReturnGenericErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var serilogLogger = Log.Logger.ForContext<ExceptionMiddleware>();
        var middleware = new ExceptionMiddleware(_ => throw new InvalidOperationException("Simulated exception"), serilogLogger);

        // Act
        await middleware.InvokeAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var result = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("An unexpected error occurred. Please try again later.", result.Message);
    }

    private class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
