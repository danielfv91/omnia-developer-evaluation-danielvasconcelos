using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Middleware;

public class ExceptionMiddlewareLogTests
{
    [Fact]
    public async Task InvokeAsync_Should_LogError_WhenExceptionOccurs()
    {
        // Arrange
        var logEvents = new List<LogEvent>();
        var sink = new DelegatingSink(e => logEvents.Add(e));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Sink(sink)
            .CreateLogger();

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var serilogLogger = Log.Logger.ForContext<ExceptionMiddleware>();
        var middleware = new ExceptionMiddleware(_ => throw new Exception("Simulated"), serilogLogger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var errorLog = logEvents.FirstOrDefault(e => e.Level == LogEventLevel.Error);
        Assert.NotNull(errorLog);
        Assert.Contains("An unhandled exception occurred", errorLog.RenderMessage());
        Assert.Contains("ExceptionMiddleware", errorLog.Properties["SourceContext"].ToString());
    }

    private class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _write;

        public DelegatingSink(Action<LogEvent> write) => _write = write;

        public void Emit(LogEvent logEvent) => _write(logEvent);
    }
}
