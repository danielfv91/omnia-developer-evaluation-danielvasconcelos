using Serilog;

namespace Ambev.DeveloperEvaluation.Common.Logging;

/// <summary>
/// Provides extension methods to add contextual log files based on Serilog's SourceContext.
/// </summary>
public static class LoggerConfigurationExtensions
{
    /// <summary>
    /// Adds multiple contextual logs using a dictionary where key = SourceContext and value = filename.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog logger configuration.</param>
    /// <param name="contextToFileMap">A dictionary mapping SourceContext names to log file names.</param>
    /// <returns>The updated logger configuration.</returns>
    public static LoggerConfiguration AddContextualLogs(
        this LoggerConfiguration loggerConfiguration,
        Dictionary<string, string> contextToFileMap)
    {
        foreach (var kvp in contextToFileMap)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(le =>
                    le.Properties.ContainsKey("SourceContext") &&
                    le.Properties["SourceContext"].ToString() == $"\"{kvp.Key}\"")
                .WriteTo.File(
                    path: $"logs/{kvp.Value}-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                ));
        }

        return loggerConfiguration;
    }
}
