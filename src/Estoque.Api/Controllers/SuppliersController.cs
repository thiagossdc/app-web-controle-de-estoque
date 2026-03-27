using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public class SuppliersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAll() =>
        Ok(await dbContext.Suppliers.OrderBy(x => x.Name).ToListAsync());

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Supplier>> Create(SupplierUpsertRequest request)
    {
        var entity = new Supplier
        {
            Name = request.Name.Trim(),
            ContactName = request.ContactName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim()
        };
        dbContext.Suppliers.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }
}
