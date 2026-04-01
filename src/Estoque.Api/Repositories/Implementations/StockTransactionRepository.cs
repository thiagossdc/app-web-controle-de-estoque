using Estoque.Api.Data;
using Estoque.Api.Domain;
using Estoque.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Repositories.Implementations;

public class StockTransactionRepository : IStockTransactionRepository
{
    private readonly AppDbContext _context;

    public StockTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockTransaction?> GetByIdAsync(int id)
    {
        return await _context.StockTransactions
            .Include(st => st.Product)
            .Include(st => st.User)
            .FirstOrDefaultAsync(st => st.Id == id);
    }

    public async Task<IEnumerable<StockTransaction>> GetAllAsync()
    {
        return await _context.StockTransactions
            .Include(st => st.Product)
            .Include(st => st.User)
            .OrderByDescending(st => st.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
    {
        return await _context.StockTransactions
            .Include(st => st.Product)
            .Include(st => st.User)
            .Where(st => st.ProductId == productId)
            .OrderByDescending(st => st.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<StockTransaction> CreateAsync(StockTransaction transaction)
    {
        _context.StockTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<int> CountLast30DaysAsync()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        return await _context.StockTransactions
            .CountAsync(st => st.CreatedAtUtc >= thirtyDaysAgo);
    }

    public async Task<IEnumerable<DailyFlow>> GetDailyFlowAsync(int days = 30)
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        
        var transactions = await _context.StockTransactions
            .Where(st => st.CreatedAtUtc >= startDate.ToDateTime(TimeOnly.MinValue))
            .GroupBy(st => DateOnly.FromDateTime(st.CreatedAtUtc))
            .Select(g => new DailyFlow(
                g.Key,
                g.Where(st => st.Type == StockTransactionType.Entrada).Sum(st => st.Quantity),
                g.Where(st => st.Type == StockTransactionType.Saida).Sum(st => st.Quantity)
            ))
            .OrderBy(df => df.Day)
            .ToListAsync();

        return transactions;
    }
}