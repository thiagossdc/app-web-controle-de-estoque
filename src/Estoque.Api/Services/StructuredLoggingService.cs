using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Estoque.Api.Services;

public interface IStructuredLoggingService
{
    void LogInformation(string message, object? data = null);
    void LogWarning(string message, object? data = null);
    void LogError(string message, Exception? exception = null, object? data = null);
    void LogDebug(string message, object? data = null);
    void LogBusinessEvent(string eventName, object? data = null);
    void LogSecurityEvent(string eventName, object? data = null);
    void LogPerformanceEvent(string eventName, double durationMs, object? data = null);
}

public class StructuredLoggingService : IStructuredLoggingService
{
    private readonly ILogger<StructuredLoggingService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public StructuredLoggingService(ILogger<StructuredLoggingService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public void LogInformation(string message, object? data = null)
    {
        var logData = CreateLogData(message, data);
        _logger.LogInformation("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogWarning(string message, object? data = null)
    {
        var logData = CreateLogData(message, data);
        _logger.LogWarning("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogError(string message, Exception? exception = null, object? data = null)
    {
        var logData = CreateLogData(message, data);
        if (exception != null)
        {
            logData["exception"] = new
            {
                type = exception.GetType().Name,
                message = exception.Message,
                stackTrace = exception.StackTrace
            };
        }
        _logger.LogError("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogDebug(string message, object? data = null)
    {
        var logData = CreateLogData(message, data);
        _logger.LogDebug("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogBusinessEvent(string eventName, object? data = null)
    {
        var logData = CreateLogData($"Business Event: {eventName}", data);
        logData["eventType"] = "business";
        logData["eventName"] = eventName;
        _logger.LogInformation("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogSecurityEvent(string eventName, object? data = null)
    {
        var logData = CreateLogData($"Security Event: {eventName}", data);
        logData["eventType"] = "security";
        logData["eventName"] = eventName;
        _logger.LogWarning("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
    }

    public void LogPerformanceEvent(string eventName, double durationMs, object? data = null)
    {
        var logData = CreateLogData($"Performance Event: {eventName}", data);
        logData["eventType"] = "performance";
        logData["eventName"] = eventName;
        logData["durationMs"] = durationMs;
        
        if (durationMs > 1000) // Log as warning if slow
        {
            _logger.LogWarning("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
        }
        else
        {
            _logger.LogInformation("{StructuredLog}", JsonSerializer.Serialize(logData, _jsonOptions));
        }
    }

    private Dictionary<string, object> CreateLogData(string message, object? data = null)
    {
        var logData = new Dictionary<string, object>
        {
            ["message"] = message,
            ["timestamp"] = DateTime.UtcNow,
            ["application"] = "Estoque.Api",
            ["version"] = "1.0.0"
        };

        if (data != null)
        {
            logData["data"] = data;
        }

        return logData;
    }
}

// Extension methods for easy logging
public static class LoggingExtensions
{
    public static void LogUserAction(this IStructuredLoggingService logger, string action, int userId, object? data = null)
    {
        logger.LogBusinessEvent(action, new { userId, data });
    }

    public static void LogProductAction(this IStructuredLoggingService logger, string action, int productId, object? data = null)
    {
        logger.LogBusinessEvent(action, new { productId, data });
    }

    public static void LogStockAction(this IStructuredLoggingService logger, string action, int productId, int quantity, object? data = null)
    {
        logger.LogBusinessEvent(action, new { productId, quantity, data });
    }

    public static void LogLoginAttempt(this IStructuredLoggingService logger, string email, bool success, object? data = null)
    {
        logger.LogSecurityEvent("LoginAttempt", new { email, success, data });
    }

    public static void LogUnauthorizedAccess(this IStructuredLoggingService logger, string endpoint, object? data = null)
    {
        logger.LogSecurityEvent("UnauthorizedAccess", new { endpoint, data });
    }
}