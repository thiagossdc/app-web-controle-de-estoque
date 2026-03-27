using System.ComponentModel.DataAnnotations;

namespace Estoque.Api.Domain;

public enum UserRole
{
    Admin = 1,
    Funcionario = 2
}

public enum StockTransactionType
{
    Entrada = 1,
    Saida = 2,
    Ajuste = 3
}

public class User
{
    public int Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(120)] public string Email { get; set; } = string.Empty;
    [MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public class Category
{
    public int Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(300)] public string? Description { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(120)] public string ContactName { get; set; } = string.Empty;
    [MaxLength(120)] public string Email { get; set; } = string.Empty;
    [MaxLength(40)] public string Phone { get; set; } = string.Empty;
}

public class Product
{
    public int Id { get; set; }
    [MaxLength(160)] public string Name { get; set; } = string.Empty;
    [MaxLength(80)] public string Sku { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public decimal UnitPrice { get; set; }
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
}

public class StockTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public StockTransactionType Type { get; set; }
    public int Quantity { get; set; }
    [MaxLength(300)] public string? Reason { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
