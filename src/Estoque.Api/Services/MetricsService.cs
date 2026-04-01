using System.Diagnostics;
using System.Diagnostics.Metrics;
using Estoque.Api.Interfaces;

namespace Estoque.Api.Services;

public class MetricsService : IMetricsService
{
    private readonly ILogger<MetricsService> _logger;
    private readonly Meter _meter;
    
    // Contadores
    private readonly Counter<long> _requestCounter;
    private readonly Counter<long> _cacheHitCounter;
    private readonly Counter<long> _cacheMissCounter;
    private readonly Counter<long> _databaseQueryCounter;
    private readonly Counter<long> _rabbitMqMessageCounter;
    
    // Histogramas
    private readonly Histogram<double> _requestDuration;
    private readonly Histogram<double> _databaseQueryDuration;
    
    // Medidores
    private readonly ObservableGauge<int> _activeConnections;
    private int _activeConnectionCount = 0;

    public MetricsService(ILogger<MetricsService> logger)
    {
        _logger = logger;
        _meter = new Meter("Estoque.Api");
        
        // Inicializar contadores
        _requestCounter = _meter.CreateCounter<long>("http_requests_total", "requests", "Total de requisições HTTP");
        _cacheHitCounter = _meter.CreateCounter<long>("cache_hits_total", "hits", "Total de acertos de cache");
        _cacheMissCounter = _meter.CreateCounter<long>("cache_misses_total", "misses", "Total de falhas de cache");
        _databaseQueryCounter = _meter.CreateCounter<long>("database_queries_total", "queries", "Total de consultas ao banco");
        _rabbitMqMessageCounter = _meter.CreateCounter<long>("rabbitmq_messages_total", "messages", "Total de mensagens RabbitMQ");
        
        // Inicializar histogramas
        _requestDuration = _meter.CreateHistogram<double>("http_request_duration_ms", "ms", "Duração da requisição HTTP");
        _databaseQueryDuration = _meter.CreateHistogram<double>("database_query_duration_ms", "ms", "Duração da consulta ao banco");
        
        // Inicializar medidores
        _activeConnections = _meter.CreateObservableGauge<int>("active_connections", () => _activeConnectionCount);
    }

    public void RecordRequestDuration(string endpoint, string method, int statusCode, double durationMs)
    {
        var tags = new TagList
        {
            { "endpoint", endpoint },
            { "method", method },
            { "status_code", statusCode }
        };
        
        _requestCounter.Add(1, tags);
        _requestDuration.Record(durationMs, tags);
        
        _logger.LogDebug("Request {Method} {Endpoint} - {StatusCode} - {Duration}ms", 
            method, endpoint, statusCode, durationMs);
    }

    public void RecordCacheHit(string key)
    {
        _cacheHitCounter.Add(1, new TagList { { "key", key } });
        _logger.LogDebug("Cache hit for key: {Key}", key);
    }

    public void RecordCacheMiss(string key)
    {
        _cacheMissCounter.Add(1, new TagList { { "key", key } });
        _logger.LogDebug("Cache miss for key: {Key}", key);
    }

    public void RecordDatabaseQuery(string query, double durationMs)
    {
        _databaseQueryCounter.Add(1, new TagList { { "query", query } });
        _databaseQueryDuration.Record(durationMs, new TagList { { "query", query } });
        
        _logger.LogDebug("Database query {Query} - {Duration}ms", query, durationMs);
    }

    public void RecordRabbitMqMessage(string queue, string operation)
    {
        _rabbitMqMessageCounter.Add(1, new TagList 
        { 
            { "queue", queue }, 
            { "operation", operation } 
        });
        
        _logger.LogDebug("RabbitMQ {Operation} on queue {Queue}", operation, queue);
    }

    public Dictionary<string, object> GetMetrics()
    {
        return new Dictionary<string, object>
        {
            ["active_connections"] = _activeConnectionCount,
            ["timestamp"] = DateTime.UtcNow
        };
    }

    public void IncrementActiveConnections()
    {
        Interlocked.Increment(ref _activeConnectionCount);
    }

    public void DecrementActiveConnections()
    {
        Interlocked.Decrement(ref _activeConnectionCount);
    }

    public void IncrementRequestCount(string endpoint, string method, int statusCode)
    {
        var tags = new TagList
        {
            { "endpoint", endpoint },
            { "method", method },
            { "status_code", statusCode }
        };
        
        _requestCounter.Add(1, tags);
        _logger.LogDebug("Request count incremented for {Method} {Endpoint} - {StatusCode}", 
            method, endpoint, statusCode);
    }

    public Task<string> GetMetricsAsync()
    {
        var metrics = GetMetrics();
        var json = System.Text.Json.JsonSerializer.Serialize(metrics);
        return Task.FromResult(json);
    }
}

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetricsService _metricsService;
    private readonly ILogger<MetricsMiddleware> _logger;

    public MetricsMiddleware(RequestDelegate next, IMetricsService metricsService, ILogger<MetricsMiddleware> logger)
    {
        _next = next;
        _metricsService = metricsService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var path = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            
            _metricsService.RecordRequestDuration(path, method, statusCode, stopwatch.ElapsedMilliseconds);
            
            if (stopwatch.ElapsedMilliseconds > 1000) // Log de requisições lentas
            {
                _logger.LogWarning("Slow request: {Method} {Path} - {StatusCode} - {Duration}ms", 
                    method, path, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}