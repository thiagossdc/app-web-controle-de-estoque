using Estoque.Api.Domain;

namespace Estoque.Api.Repositories.Interfaces;

public interface IStockTransactionRepository
{
    Task<StockTransaction?> GetByIdAsync(int id);
    Task<IEnumerable<StockTransaction>> GetAllAsync();
    Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId);
    Task<StockTransaction> CreateAsync(StockTransaction transaction);
    Task<int> CountLast30DaysAsync();
    Task<IEnumerable<DailyFlow>> GetDailyFlowAsync(int days = 30);
}

public record DailyFlow(DateOnly Day, int Entradas, int Saidas);