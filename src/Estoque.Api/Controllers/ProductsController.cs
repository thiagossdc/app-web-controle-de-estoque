using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<Product>>> GetAll([FromQuery] ProductQueryRequest query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 100);

        var productsQuery = dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            productsQuery = productsQuery.Where(x => x.Name.ToLower().Contains(search) || x.Sku.ToLower().Contains(search));
        }

        var totalItems = await productsQuery.CountAsync();
        var items = await productsQuery
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResponse<Product>(items, totalItems, page, pageSize));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Product>> Create(ProductUpsertRequest request)
    {
        var entity = new Product
        {
            Name = request.Name.Trim(),
            Sku = request.Sku.Trim(),
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            UnitPrice = request.UnitPrice,
            MinStock = request.MinStock,
            CurrentStock = 0
        };
        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }
}
