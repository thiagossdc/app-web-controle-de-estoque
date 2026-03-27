using Estoque.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(x => x.Sku).IsUnique();

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Eletronicos", Description = "Produtos de tecnologia" },
            new Category { Id = 2, Name = "Escritorio", Description = "Itens corporativos" }
        );

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "Global Distribuicao", ContactName = "Fernanda Lima", Email = "contato@globaldist.com", Phone = "11999999999" },
            new Supplier { Id = 2, Name = "Pro Office Supplies", ContactName = "Carlos Souza", Email = "vendas@prooffice.com", Phone = "11888888888" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Notebook Corporativo 14", Sku = "NB-14-CP", CategoryId = 1, SupplierId = 1, UnitPrice = 4200m, CurrentStock = 25, MinStock = 10 },
            new Product { Id = 2, Name = "Cadeira Ergonomica", Sku = "CAD-ERG-01", CategoryId = 2, SupplierId = 2, UnitPrice = 980m, CurrentStock = 40, MinStock = 12 }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Administrador", Email = "admin@estoque.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = UserRole.Admin },
            new User { Id = 2, Name = "Funcionario Padrao", Email = "funcionario@estoque.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Funcionario@123"), Role = UserRole.Funcionario }
        );
    }
}
