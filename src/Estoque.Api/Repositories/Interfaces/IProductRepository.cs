using Estoque.Api.Contracts;
using Estoque.Api.Domain;

namespace Estoque.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<PagedResponse<Product>> GetAllAsync(ProductQueryRequest query);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}