using Estoque.Api.Contracts;
using FluentValidation;

namespace Estoque.Api.Validators;

public class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
{
    public ProductUpsertRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(160).WithMessage("Nome deve ter no máximo 160 caracteres.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU é obrigatório.")
            .MaximumLength(80).WithMessage("SKU deve ter no máximo 80 caracteres.")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("SKU deve conter apenas letras, números e hífen.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Categoria é obrigatória.");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Fornecedor é obrigatório.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Preço unitário deve ser maior que zero.");

        RuleFor(x => x.MinStock)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo deve ser maior ou igual a zero.");
    }
}

public class ProductQueryRequestValidator : AbstractValidator<ProductQueryRequest>
{
    public ProductQueryRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Tamanho da página deve ser maior que zero.")
            .LessThanOrEqualTo(100).WithMessage("Tamanho da página deve ser no máximo 100.");

        RuleFor(x => x.Search)
            .MaximumLength(100).WithMessage("Busca deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}