using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardReportResponse>> Dashboard()
    {
        var last30 = DateTime.UtcNow.AddDays(-30);
        var transactions = await dbContext.StockTransactions
            .Where(x => x.CreatedAtUtc >= last30)
            .ToListAsync();

        var dailyFlow = transactions
            .GroupBy(x => DateOnly.FromDateTime(x.CreatedAtUtc.Date))
            .Select(g => new DailyFlow(
                g.Key,
                g.Where(x => x.Type == StockTransactionType.Entrada).Sum(x => x.Quantity),
                g.Where(x => x.Type == StockTransactionType.Saida).Sum(x => x.Quantity)))
            .OrderBy(x => x.Day)
            .ToList();

        var response = new DashboardReportResponse(
            TotalProducts: await dbContext.Products.CountAsync(),
            TotalCategories: await dbContext.Categories.CountAsync(),
            LowStockProducts: await dbContext.Products.CountAsync(x => x.CurrentStock <= x.MinStock),
            TransactionsLast30Days: transactions.Count,
            DailyFlow: dailyFlow);

        return Ok(response);
    }
}
