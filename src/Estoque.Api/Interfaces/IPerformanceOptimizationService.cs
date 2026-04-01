namespace Estoque.Api.Interfaces;

public interface IPerformanceOptimizationService
{
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, TimeSpan? delay = null);
    Task ExecuteWithRetryAsync(Func<Task> operation, int maxRetries = 3, TimeSpan? delay = null);
    Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> operation, TimeSpan timeout);
    Task ExecuteWithTimeoutAsync(Func<Task> operation, TimeSpan timeout);
}