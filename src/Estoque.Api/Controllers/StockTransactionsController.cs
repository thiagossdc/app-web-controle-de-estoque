using System.Security.Claims;
using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Estoque.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/stock-transactions")]
[Authorize]
public class StockTransactionsController(AppDbContext dbContext, InventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockTransaction>>> GetAll()
    {
        var data = await dbContext.StockTransactions
            .Include(x => x.Product)
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<StockTransaction>> Create(StockTransactionRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var transaction = await inventoryService.RegisterTransactionAsync(userId, request);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
