using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll() =>
        Ok(await dbContext.Categories.OrderBy(x => x.Name).ToListAsync());

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Category>> Create(CategoryUpsertRequest request)
    {
        var entity = new Category { Name = request.Name.Trim(), Description = request.Description?.Trim() };
        dbContext.Categories.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }
}
