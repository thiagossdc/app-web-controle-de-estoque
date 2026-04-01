namespace Estoque.Api.Interfaces;

public interface IMetricsService
{
    void RecordRequestDuration(string endpoint, string method, int statusCode, double durationMs);
    void IncrementRequestCount(string endpoint, string method, int statusCode);
    Task<string> GetMetricsAsync();
}