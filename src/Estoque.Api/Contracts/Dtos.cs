using Estoque.Api.Domain;

namespace Estoque.Api.Contracts;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string AccessToken, string UserName, string Role);

public record CategoryUpsertRequest(string Name, string? Description);
public record SupplierUpsertRequest(string Name, string ContactName, string Email, string Phone);
public record ProductUpsertRequest(string Name, string Sku, int CategoryId, int SupplierId, decimal UnitPrice, int MinStock);

public record StockTransactionRequest(int ProductId, StockTransactionType Type, int Quantity, string? Reason);
public record ProductQueryRequest(string? Search, int Page = 1, int PageSize = 20);
public record PagedResponse<T>(IEnumerable<T> Items, int TotalItems, int Page, int PageSize);

public record DashboardReportResponse(
    int TotalProducts,
    int TotalCategories,
    int LowStockProducts,
    int TransactionsLast30Days,
    IEnumerable<DailyFlow> DailyFlow
);

public record DailyFlow(DateOnly Day, int Entradas, int Saidas);
