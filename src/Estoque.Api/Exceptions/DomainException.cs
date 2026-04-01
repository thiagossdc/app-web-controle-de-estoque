namespace Estoque.Api.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int id) : base($"Produto com ID {id} não encontrado.") { }
}

public class CategoryNotFoundException : DomainException
{
    public CategoryNotFoundException(int id) : base($"Categoria com ID {id} não encontrada.") { }
}

public class SupplierNotFoundException : DomainException
{
    public SupplierNotFoundException(int id) : base($"Fornecedor com ID {id} não encontrado.") { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(int id) : base($"Usuário com ID {id} não encontrado.") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(int productId, int requested, int available) 
        : base($"Estoque insuficiente para o produto {productId}. Solicitado: {requested}, Disponível: {available}.") { }
}

public class InvalidStockOperationException : DomainException
{
    public InvalidStockOperationException(string message) : base(message) { }
}

public class DuplicateEmailException : DomainException
{
    public DuplicateEmailException(string email) : base($"Email {email} já está em uso.") { }
}

public class DuplicateSkuException : DomainException
{
    public DuplicateSkuException(string sku) : base($"SKU {sku} já está em uso.") { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException() : base("Acesso não autorizado.") { }
    public UnauthorizedException(string message) : base(message) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException() : base("Acesso proibido.") { }
    public ForbiddenException(string message) : base(message) { }
}