using Estoque.Api.Contracts;
using Estoque.Api.Domain;

namespace Estoque.Api.Interfaces;

public interface IInventoryService
{
    Task<StockTransaction> RegisterTransactionAsync(int userId, StockTransactionRequest request);
    Task<IEnumerable<StockTransaction>> GetTransactionsAsync();
    Task<IEnumerable<StockTransaction>> GetTransactionsByProductAsync(int productId);
}